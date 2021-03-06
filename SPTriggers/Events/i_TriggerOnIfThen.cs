﻿#pragma warning disable 0649 // variable declared but not used.

using UnityEngine;

using com.spacepuppy.Utils;

namespace com.spacepuppy.Events
{

    public class i_TriggerOnIfThen : AutoTriggerable
    {

        #region Fields

        [SerializeField()]
        private ConditionBlock[] _conditions;
        [SerializeField]
        private SPEvent _elseCondition;

        [SerializeField()]
        private bool _passAlongTriggerArg;

        [SerializeField()]
        private SPTimePeriod _delay = 0f;

        #endregion

        #region Properties

        public bool PassAlongTriggerArg
        {
            get { return _passAlongTriggerArg; }
            set { _passAlongTriggerArg = value; }
        }

        public SPTimePeriod Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        #endregion

        #region ITriggerableMechanism Interface

        public override bool Trigger(object sender, object arg)
        {
            if (_conditions == null || _conditions.Length == 0) return false;

            if (!this._passAlongTriggerArg) arg = null;
            foreach (var c in _conditions)
            {
                if (c.Condition.BoolValue)
                {
                    if (_delay.Seconds > 0f)
                    {
                        this.InvokeGuaranteed(() =>
                        {
                            c.Trigger.ActivateTrigger(this, arg);
                        }, _delay.Seconds, _delay.TimeSupplier);
                    }
                    else
                    {
                        c.Trigger.ActivateTrigger(this, arg);
                    }
                    return true;
                }
            }

            //if we reached here, it's else
            if (_delay.Seconds > 0f)
            {
                this.InvokeGuaranteed(() =>
                {
                    _elseCondition.ActivateTrigger(this, arg);
                }, _delay.Seconds, _delay.TimeSupplier);
            }
            else
            {
                _elseCondition.ActivateTrigger(this, arg);
            }
            return true;
        }

        #endregion

        #region Special Types

        [System.Serializable()]
        public class ConditionBlock
        {

            [SerializeField()]
            private VariantReference _condition = new VariantReference();
            [SerializeField()]
            private SPEvent _trigger = new SPEvent();


            public VariantReference Condition
            {
                get { return _condition; }
            }

            public SPEvent Trigger
            {
                get { return _trigger; }
            }

        }

        #endregion

    }

}
