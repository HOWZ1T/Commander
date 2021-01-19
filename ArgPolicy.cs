namespace Commander
{
    public enum Policy
    {
        Exact,
        Min,
        Max,
        Between,
        None,
        Any
    }
    
    public class ArgPolicy
    {
        private int _min, _max;
        private Policy _policy;
        
        public ArgPolicy(int v, int v1, Policy policy = Policy.Between)
        {
            this._min = v;
            this._max = v1;
            this._policy = policy;
        }
        
        public ArgPolicy(int v, Policy policy = Policy.Exact)
        {
            if (policy == Policy.Min)
            {
                this._min = v;
                this._max = 0;
            } else if (policy == Policy.Max)
            {
                this._min = 0;
                this._max = v;
            }
            else if (policy == Policy.Between)
            {
                this._min = v;
                this._max = v;
            }
            else
            {
                this._min = v;
                this._max = 0;
            }
            
            this._policy = policy;
        }

        public ArgPolicy(Policy policy = Policy.None)
        {
            this._min = 0;
            this._max = 0;
            this._policy = policy;
        }

        public bool isValid(int argCount)
        {
            switch (this._policy)
            {
                case Policy.Exact:
                    if (argCount != this._min)
                    {
                        return false;
                    }
                    return true;
                
                case Policy.Min:
                    if (argCount < this._min)
                    {
                        return false;
                    }
                    return true;
                
                case Policy.Max:
                    if (argCount > this._max)
                    {
                        return false;
                    }
                    return true;
                
                case Policy.Between:
                    if (argCount < this._min || argCount > this._max)
                    {
                        return false;
                    }
                    return true;
                
                case Policy.None:
                    if (argCount != 0)
                    {
                        return false;
                    }
                    return true;
                
                case Policy.Any:
                    return true;
                
                default:
                    return false;
            }
        }
    }
}