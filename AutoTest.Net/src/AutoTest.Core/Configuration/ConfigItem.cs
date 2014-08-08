using System;
namespace AutoTest.Core.Configuration
{
	public class ConfigItem<T>
	{
		public bool WasReadFromConfig { get; private set; }
		public bool ShouldMerge { get; private set; }
		public bool ShouldExclude { get; private set; }
		public T Value { get; private set; }
		
		public ConfigItem(T defaultValue)
		{
			Value = defaultValue;
			WasReadFromConfig = false;
			ShouldMerge = false;
			ShouldExclude = false;
		}
		
		public ConfigItem<T> SetValue(T newValue)
		{
			Value = newValue;
			WasReadFromConfig = true;
			ShouldExclude = false;
			return this;
		}
		
		public void SetShouldMerge()
		{
			ShouldMerge = true;
			ShouldExclude = false;
		}
		
		public void Exclude()
		{
			ShouldExclude = true;
			ShouldMerge = false;
		}
	}
}

