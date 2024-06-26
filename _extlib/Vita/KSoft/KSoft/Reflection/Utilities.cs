﻿using System;
using System.Collections.Generic;
using System.Linq;
using Contracts = System.Diagnostics.Contracts;
#if CONTRACTS_FULL_SHIM
using Contract = System.Diagnostics.ContractsShim.Contract;
#else
using Contract = System.Diagnostics.Contracts.Contract; // SHIM'D
#endif
using Reflect = System.Reflection;
using Interop = System.Runtime.InteropServices;

namespace KSoft.Reflection
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1724:TypeNamesShouldNotMatchNamespaces",
		Justification="I don't care about System.Web.Util")]
	public static partial class Util
	{
		[Contracts.Pure]
		public static bool IsEnumType(object maybeType) =>
			maybeType is Type type && type.IsEnum;

		[Contracts.Pure]
		public static bool IsEnumTypeOrNull(object maybeType) =>
			maybeType == null || IsEnumType(maybeType);

		[Contracts.Pure]
		public static List<Reflect.FieldInfo> GetEnumFields(Type enumType)
		{
			Contract.Requires<ArgumentNullException>(enumType != null);
			Contract.Requires<ArgumentException>(enumType.IsEnum);
			Contract.Ensures(Contract.Result<List<Reflect.FieldInfo>>() != null);

			return EnumUtils.GetEnumFields(enumType);
		}

		const string kDelegateInvokeMethodName = "Invoke";
		// http://www.codeproject.com/Tips/441743/A-look-at-marshalling-delegates-in-NET
		public static T GetDelegateForFunctionPointer<T>(IntPtr nativePtr, Interop.CallingConvention callConv)
			where T : class
		{
			Contract.Requires<ArgumentException>(typeof(T).IsSubclassOf(typeof(Delegate)));
			Contract.Requires<ArgumentNullException>(nativePtr != IntPtr.Zero);
			Contract.Requires<ArgumentException>(callConv != Interop.CallingConvention.ThisCall,
				"TODO: ThisCall's require a different implementation"); // #TODO

			Contract.Ensures(Contract.Result<T>() != null);

			var type = typeof(T);
			var method = type.GetMethod(kDelegateInvokeMethodName);
			var ret_type = method.ReturnType;
			var param_types = (from param in method.GetParameters()
							   select param.ParameterType)
							  .ToArray();

			var invoke = new Reflect.Emit.DynamicMethod(kDelegateInvokeMethodName, ret_type, param_types,
				typeof(Delegate));
			var il = invoke.GetILGenerator();

			// Generate IL for loading all the args by index
			// #REVIEW: IL has Ldarg_0 to Ldarg_3...do these provide any tangible perf benefits?
			{
				int arg_index = 0;
				if (param_types.Length >= 1)
				{
					il.Emit(Reflect.Emit.OpCodes.Ldarg_0);
					arg_index++;
				}
				if (param_types.Length >= 2)
				{
					il.Emit(Reflect.Emit.OpCodes.Ldarg_1);
					arg_index++;
				}
				if (param_types.Length >= 3)
				{
					il.Emit(Reflect.Emit.OpCodes.Ldarg_2);
					arg_index++;
				}
				if (param_types.Length >= 4)
				{
					il.Emit(Reflect.Emit.OpCodes.Ldarg_3);
					arg_index++;
				}

				for (int x = arg_index; x < param_types.Length; x++)
					il.Emit(Reflect.Emit.OpCodes.Ldarg, x);
			}

			// Generate the IL for Calli's entry pointer (pushed to the stack)
			if (Environment.Is64BitProcess)
				il.Emit(Reflect.Emit.OpCodes.Ldc_I8, nativePtr.ToInt64());
			else
				il.Emit(Reflect.Emit.OpCodes.Ldc_I4, nativePtr.ToInt32());

			il.EmitCalli(Reflect.Emit.OpCodes.Calli, callConv, ret_type, param_types);
			il.Emit(Reflect.Emit.OpCodes.Ret);

			return invoke.CreateDelegate(type) as T;
		}
	};
}
