﻿using System;

namespace SilentHunter
{
	[Serializable]
	// ReSharper disable once InconsistentNaming
	public struct SHUnion<TTypeA, TTypeB> : ICloneable
		where TTypeA : struct
		where TTypeB : struct
	{
		private object _value;
		private Type _currentType;

		public Type Type
		{
			get => _currentType ?? typeof(TTypeA);
			set
			{
				if (value == _currentType)
				{
					return;
				}

				if (value == typeof(TTypeA) || value == typeof(TTypeB))
				{
					_currentType = value;
					if (_value != null)
					{
						_value = Convert.ChangeType(_value, _currentType);
					}
				}
				else
				{
					throw new ArgumentException($"The specified type is not valid. This union only supports types {typeof(TTypeA).Name} and {typeof(TTypeB).Name}.", nameof(value));
				}
			}
		}

		public object Value
		{
			get
			{
				if (_value != null)
				{
					return _value;
				}

				return Type == typeof(TTypeA)
					? (object)default(TTypeA)
					: default(TTypeB);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				if (value.GetType() != Type)
				{
					throw new ArgumentException("The specified value is of invalid type.", nameof(value));
				}

				_value = value;
			}
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public object Clone()
		{
			var clone = new SHUnion<TTypeA, TTypeB>
			{
				_currentType = _currentType,
				_value = _value
			};
			return clone;
		}
	}
}