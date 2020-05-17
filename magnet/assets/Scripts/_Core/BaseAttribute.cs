using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

namespace Kpro
{
    public delegate void ValueChangedHandler(BaseAttribute attribute);
  
    /// <summary>
    /// 모든 스탯에 기본이 되는 클래스
    /// </summary>
    public class BaseAttribute
    {
        public event ValueChangedHandler ValueChanged;

        private float _value;
        private float _multiplier;

        //< 계산의 우선순위를 주기위한 수치
        public int Depth = 0;

        /// <summary>
        /// 이름구분용
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 가장 기본값
        /// </summary>
        public float Value 
        {
            set {
                _value = value;
                OnValueChanged();
            }
            get { return _value; }
        }

        /// <summary>
        /// 곱하는 비율 (1f == 100%)
        /// </summary>
        public float Multiplier
        {
            set {
                if (_multiplier != value)
                    _multiplier = value;
                OnValueChanged();
            }
            get { return _multiplier; }
        }
    
        public BaseAttribute(float initValue = 0, float initMultiplier = 0f, string name = "", int _depth = 0)
        {
            Value = initValue;
            Multiplier = initMultiplier;
            Name = name;
            Depth = _depth;
        }

        /// <summary>
        /// ex) 스킬 중복 적용에 의한 수치 변경시, 소유자에게 변화를 알리기 위함,
        ///     소유자는 변경된 값을 기반으로 다시 최종값을 만들어내야함.
        /// </summary>
        protected void OnValueChanged()
        {
            if (null != ValueChanged)
                ValueChanged(this);
        }
    }

    public class RawValue : BaseAttribute
    {
        public RawValue(float initValue = 0, float initMultiplier = 0f, string name = "") : base(initValue, initMultiplier, name)
        { }
    }

    public class FinalValue : BaseAttribute
    {
        public FinalValue(float initValue = 0, float initMultiplier = 0f, string name = "") : base(initValue, initMultiplier, name)
        { }
    }

    /// <summary>
    /// 다양한 상황에 맞게 사용가능한 스탯 클래스
    /// </summary>
    /// <remarks>
    /// 기본 능력치가 존재하면서, 추가로 각종 버프, 추가 능력치 적용이 필요한 스탯에 사용하면 됩니다.
    /// ex). 최종 공격력 계산하기
    ///     아이템 기본 공격력 : 10
    ///     +아이템 옵션 공격력 : 20 = 30
    ///     +아이템 옵셥 공격력% : 10% = 31
    ///     +아이템 최종 공격력% : 10% = 34.1
    ///     ======
    ///     최종 => 34.1
    /// </remarks>
    public class Attribute : BaseAttribute
    {
        List<BaseAttribute> rawBonuses;
        List<BaseAttribute> finalBonuses;

        /// <summary>
        /// 최종값 변경시에 불려서 수행해야될 이벤트 핸들러.
        /// </summary>
        public event ValueChangedHandler FinalValueChanged;

        public float FinalValue 
        { 
            protected set{}
            get 
            { 
                if (_FinalValue.ContainsKey(0)) 
                    return _FinalValue[0];

                return 0;
            } 
        }

        Dictionary<int, float> _FinalValue = new Dictionary<int, float>();

        public float GetFinalValue(int depth)
        {
            if (_FinalValue.ContainsKey(depth))
                return _FinalValue[depth];

            return 0;
        }

        public float GetTotalFinalValue()
        {
            float value = 0;
            foreach (KeyValuePair<int, float> dic in _FinalValue)
                value += dic.Value;

            return value;
        }

        public Attribute(float startValue, float multiplier = 0f, string name = "", int _depth = 0)
            : base(startValue, multiplier, name, _depth)
        {
            rawBonuses = new List<BaseAttribute>();
            finalBonuses = new List<BaseAttribute>();

            ValueChanged += ChangeAttribute;

            CalculateValue();
        }

        public void Add(BaseAttribute attribute)
        {
            if (null == attribute)
                return;

            //< 혹시 Multiplier 값이 들어있다면 띄워줌(들어있으면 안되는값이기에)
            if (attribute.Multiplier > 0)
                Debug.Log(">>>>> attribute.Multiplier 값이 0보다 큼!! [" + attribute.Name + "] <<<<<");

            attribute.Multiplier = 0;

            if (attribute is FinalValue)
                finalBonuses.Add(attribute);
            else
                // BaseAttribute도 RawBonuses로 귀속되도록 한다.
                rawBonuses.Add(attribute);

            attribute.ValueChanged += ChangeAttribute;

            OnValueChanged();
        }

        public bool Remove(BaseAttribute attribute)
        {
            if (null == attribute)
                return false;

            bool removed = false;

            if (attribute is FinalValue)
                removed = finalBonuses.Remove( attribute );
            else
                // BaseAttribute도 RawBonuses로 귀속되도록 한다.
                removed = rawBonuses.Remove( attribute );

            attribute.ValueChanged -= ChangeAttribute;

            OnValueChanged();

            return removed;
        }

        /// <summary>
        /// 이름에 맞는 Attribute를 찾아준다.
        /// </summary>
        /// <param name="name"></param>
        public BaseAttribute Find(string name)
        {
            BaseAttribute foundAttr = rawBonuses.Find( (attr) => attr.Name == name );
            if (null == foundAttr)
                foundAttr = finalBonuses.Find( (attr) => attr.Name == name );

            return foundAttr;
        }

        public float CalculateValue()
        {
            for (int i = 0; i < 3; i++ )
            {
                if(_FinalValue.ContainsKey(i))
                    _FinalValue[i] = Depth == i ? Value : 0;
            }

            if (!_FinalValue.ContainsKey(Depth))
                _FinalValue.Add(Depth, Value);

            // 기본 값 계산
            Dictionary<int, float> rawBonus = new Dictionary<int, float>();
            foreach (BaseAttribute attr in rawBonuses)
            {
                if (!rawBonus.ContainsKey(attr.Depth))
                    rawBonus.Add(attr.Depth, 0);

                rawBonus[attr.Depth] += attr.Value;
            }

            //< 뎁스별로 저장한다.
            foreach (KeyValuePair<int, float> val in rawBonus)
            {
                if (!_FinalValue.ContainsKey(val.Key))
                    _FinalValue.Add(val.Key, 0);

                _FinalValue[val.Key] += val.Value;
            }


            //< FinalValue 라는 클래스를 아예 사용하질않고있으므로 일단 제외
            // 최종 값 계산
            //float finalBonus = 0;
            //float finalMuliplier = 0;

            //foreach (BaseAttribute attr in finalBonuses)
            //{
            //    finalBonus += attr.Value;
            //    finalMuliplier += attr.Multiplier;
            //}
            //FinalValue += finalBonus;
            //FinalValue *= (1 + finalMuliplier);

            if (null != FinalValueChanged)
                FinalValueChanged( this );

            return FinalValue;
        }

        /// <summary>
        /// [called by BaseAttribute] 소속된 Attribute중에 값이 변경된다면, FinalValue는 새로 계산됨.
        /// </summary>
        void ChangeAttribute(BaseAttribute changeAttribute)
        {
            CalculateValue();
        }

        public string Print()
        {
            string result = string.Empty;
            foreach (BaseAttribute attr in rawBonuses)
            {
                result += string.Format("\n\t\t" + attr.Name + ".[RAW] Val : {0}, Mul : {1}", attr.Value, attr.Multiplier);
            }

            foreach (BaseAttribute attr in finalBonuses)
            {
                result += string.Format("\n\t\t" + attr.Name + ".[FINAL] Val : {0}, Mul : {1}", attr.Value, attr.Multiplier);
            }

            return result;
        }

        public BaseAttribute GetValue(string name)
        {
            BaseAttribute value = null;
            foreach (BaseAttribute attr in rawBonuses)
            {
                if (attr.Name == name)
                {
                    value = attr;
                    break;
                }
            }

            return value;
        }
    }

    public class OrderAttribute : BaseAttribute
    {
        public OrderAttribute(float startValue, float multiplier = 0f, string name = "") : base(startValue, multiplier, name)
        {   
        }
    }
}