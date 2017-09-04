using System.Collections.Generic;
using System.Text;
using Jurassic;
using Jurassic.Library;

namespace restFixture.Net.Javascript
{
    /// <summary>
    /// Constructor for Javascript response object.
    /// </summary>
    /// <remarks>To create an object in Javascript from .NET requires two classes in Jurassic: An 
    /// Instance class that will be the prototype for the object, and a Constructor class that will 
    /// construct the instance.</remarks>
    public class JsResponseConstructor : ClrFunction
    {
        public JsResponseConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype,
                  "JsResponse",
                  new JsResponseInstance(engine.Object.InstancePrototype))
        {
        }

        /// <summary>
        /// Represents the constructor in Javascript.
        /// </summary>
        /// <returns>A javascript instance object.</returns>
        /// <remarks>Any parameters added to this method will need to be supplied when calling the 
        /// constructor in Javascript.</remarks>
        [JSConstructorFunction]
        public JsResponseInstance Construct()
        {
            return new JsResponseInstance(this.InstancePrototype);
        }
    }

    /// <summary>
    /// Class that will be instantiated as a Javascript response object.
    /// </summary>
    public class JsResponseInstance : ObjectInstance
    {
        private IDictionary<string, IList<string>> _headers;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="prototype">Unlike other Jurrasic constructors this one takes a prototype 
        /// argument rather than a ScriptEngine argument.</param>
        public JsResponseInstance(ObjectInstance prototype)
            : base(prototype)
        {
            this.PopulateFunctions();
            _headers = new Dictionary<string, IList<string>>();
        }

        /// <param name="name">  the header name </param>
        /// <param name="value"> the value </param>
        [JSFunction(Name = "addHeader")]
        public virtual void addHeader(string name, string value)
        {
            IList<string> vals = GetHeaderValueOrDefault(name);
            if (vals == null)
            {
                vals = new List<string>();
                _headers[name] = vals;
            }
            vals.Add(value);
        }

        /// <param name="name">  the header name </param>
        /// <param name="value"> the value </param>
        [JSFunction(Name = "putHeader")]
        public virtual void putHeader(string name, string value)
        {
            IList<string> vals = new List<string>();
            vals.Add(value);
            _headers[name] = vals;
        }

        /// <param name="name"> the header name </param>
        /// <returns> the headers list size </returns>
        [JSFunction(Name = "headerListSize")]
        public virtual int headerListSize(string name)
        {
            IList<string> vals = GetHeaderValueOrDefault(name);
            if (vals == null || vals.Count == 0)
            {
                return 0;
            }
            return vals.Count;
        }

        /// <returns> the total number of headers in the response. </returns>
        [JSFunction(Name = "headersSize")]
        public virtual int headersSize()
        {
            int sz = 0;
            foreach (IList<string> hList in _headers.Values)
            {
                sz += hList.Count;
            }
            return sz;
        }

        /// <param name="name"> the header name </param>
        /// <returns> the value of the header name in position 0 </returns>
        [JSFunction(Name = "header0")]
        public virtual string header0(string name)
        {
            return header(name, 0);
        }

        /// <param name="name"> the header name </param>
        /// <returns> all headers with the given name </returns>
        [JSFunction(Name = "headersText")]
        public virtual string headersText(string name)
        {
            int sz = headerListSize(name);
            if (sz == 0)
            {
                return string.Empty;
            }
            if (sz == 1)
            {
                return header0(name);
            }
            else
            {
                StringBuilder sb = new StringBuilder("[");
                bool isFirstPass = true;
                foreach (string header in _headers[name])
                {
                    if (!isFirstPass)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(header);
                    isFirstPass = false;
                }
                sb.Append("]");
                return sb.ToString();
            }
        }

        /// <param name="name"> the header name </param>
        /// <param name="pos">  the pos </param>
        /// <returns> the value of the header with name at pos 0 </returns>
        [JSFunction(Name = "header")]
        public virtual string header(string name, int pos)
        {
            if (headerListSize(name) > 0)
            {
                return _headers[name][pos];
            }
            else
            {
                return null;
            }
        }

        /// <param name="word1">First word to concatenate.</param>
        /// <param name="word2">Second word to concatenate.</param>
        /// <returns>The two words concatenated with a space between.</returns>
        [JSFunction(Name = "test")]
        public virtual string test(string word1, string word2)
        {
            return word1 + " " + word2;
        }

        private IList<string> GetHeaderValueOrDefault(string name)
        {
            IList<string> vals;
            bool exists = _headers.TryGetValue(name, out vals);
            if (!exists)
            {
                return null;
            }
            return vals;
        }
    }
}