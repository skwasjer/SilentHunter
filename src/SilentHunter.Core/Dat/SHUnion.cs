using System;

namespace SilentHunter.Dat
{
	[Serializable]
	public class SHUnion<TypeA, TypeB>
		: ICloneable
		where TypeA : struct
		where TypeB: struct
	{
		private object _value;
		private Type _currentType;

		public SHUnion()
		{
			_currentType = typeof(TypeA);
		}

		public Type Type
		{
			get 
			{
//				if (_currentType == null) _currentType = typeof(TypeA);
				return _currentType; 
			}
			set 
			{
				if (value == _currentType) return;
				if ((value == typeof(TypeA)) || (value == typeof(TypeB)))
				{
					_currentType = value;
					if (_value != null)
						_value = Convert.ChangeType(_value, _currentType);
				}
				else
					throw new ArgumentException(string.Format("The specified type is not valid. This union only supports types {0} and {1}.", typeof(TypeA).Name, typeof(TypeB).Name), "value");
			}
		}

		public object Value
		{
			get 
			{
				if (_value == null)
				{
					if (_currentType == typeof(TypeA))
						return default(TypeA);
					else
						return default(TypeB);
				}
				return _value; 
			}
			set 
			{
				if (value == null)
					throw new ArgumentNullException("value");
				if (value.GetType() != Type)
					throw new ArgumentException("The specified value is of invalid type.", "value");
				_value = value; 
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public object Clone()
		{
			var clone = new SHUnion<TypeA, TypeB>();
			clone._currentType = _currentType;
			clone._value = _value;
			return clone;
		}
	}

}
