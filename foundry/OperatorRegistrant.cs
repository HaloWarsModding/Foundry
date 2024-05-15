using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry
{
    public class Operator
    {
        public Operator(string name)
        {
            Name = name;
            _parent = null;
            _children = new List<Operator>();
        }

        public void Activate()
        {
            OperatorActivatedArgs args = new OperatorActivatedArgs()
            {

            };
            OperatorActivated?.Invoke(this, args);
        }
        public class OperatorActivatedArgs
        {

        }
        public event EventHandler<OperatorActivatedArgs> OperatorActivated;

        public string Name { get; private set; }

        public Operator Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent != null)
                {
                    _parent._children.Remove(this);
                }

                _parent = value;

                if (value != null)
                {
                    value._children.Add(this);
                }
            }
        }
        private Operator _parent;
        public IReadOnlyCollection<Operator> Children
        {
            get
            {
                return _children.AsReadOnly();
            }
        }
        private List<Operator> _children;
    }

    public class OperatorRegistrant
    {
        public List<Operator> Operators { get; private set; } = new List<Operator>();
        public void AddOperator(Operator op)
        {
            Operators.Add(op);

            OperatorAddedArgs args = new OperatorAddedArgs()
            {
                Operator = op
            };
            OperatorAdded?.Invoke(this, args);
        }
        public void RemoveOperator(Operator op)
        {
            if (Operators.Remove(op))
            {
                OperatorRemovedArgs args = new OperatorRemovedArgs()
                {
                    Operator = op
                };
                OperatorRemoved?.Invoke(this, args);
            }
        }

        public class OperatorAddedArgs
        {
            public Operator Operator { get; set; }
        }
        public event EventHandler<OperatorAddedArgs> OperatorAdded;
        public class OperatorRemovedArgs
        {
            public Operator Operator { get; set; }
        }
        public event EventHandler<OperatorRemovedArgs> OperatorRemoved;
    }
}
