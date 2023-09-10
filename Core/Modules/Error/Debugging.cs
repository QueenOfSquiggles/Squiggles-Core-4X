using Godot;

namespace queen.error;

public static class Debugging
{

    public static bool Assert(bool value, string message = "Assertion Failed")
    {
        #if DEBUG
        // only executes in debug builds. Otherwise acts as a defensive programming style        
        if (value) return value;
        Print.Error(message);

        #endif
        return value;
    }

}