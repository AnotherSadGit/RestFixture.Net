using System.Collections.Generic;
using System.Reflection;
using Jurassic;
using Jurassic.Library;

namespace restFixture.Net.Support
{
    /// <summary>
    /// A wrapper around Variables to allow them to be accessed from JavaScript.
    /// </summary>
    /// <remarks>For the properties of a .NET class to be available in JavaScript the class must 
    /// inherit from Jurassic.Library.ObjectInstance.  The properties must also be passed into the 
    /// indexer of the class, inherited from ObjectInstance.</remarks>
    public class VariablesJavaScriptWrapper : ObjectInstance
    {
        public VariablesJavaScriptWrapper(ScriptEngine engine)
            : base(engine)
        {
        }

        public VariablesJavaScriptWrapper(ScriptEngine engine, Variables variables)
            : base(engine)
        {
            ReadVariablesIn(variables);
        }

        private void ReadVariablesIn(Variables variables)
        {
            if (variables == null || variables.Items == null)
            {
                return;
            }

            foreach (string key in variables.Items.Keys)
            {
                // ObjectInstance base class has an indexer.  Need to pass the variables into the 
                //  indexer for JavaScript to access them.
                this[key] = variables.Items[key];
            }
        }
    }
}