using System.Collections.Generic;

public static class BreakpointHelper
{
    private static Dictionary<string, object> _previousValues = new Dictionary<string, object>();

    public static bool HasChanged(string varName, object currentValue)
    {
        if (!_previousValues.ContainsKey(varName))
        {
            _previousValues[varName] = currentValue;
            return false;
        }

        if (!Equals(_previousValues[varName], currentValue))
        {
            _previousValues[varName] = currentValue;
            return true;
        }

        return false;
    }
}