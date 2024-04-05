using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace CSharpExtensions.Dynamic
{
    //https://learn.microsoft.com/en-us/archive/blogs/csharpfaq/dynamic-in-c-4-0-creating-wrappers-with-dynamicobject
    
    public class DynamicModel : DynamicObject
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        private readonly string[] _allowedFields;

        public DynamicModel()
        {
            _allowedFields = new string[] { };
        }
        
        public DynamicModel(string[] allowedFields)
        {
            _allowedFields = allowedFields;
        }
        
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (!_allowedFields.Any())
            {
                _dictionary[binder.Name] = value;
                return true;
            }

            // Only add the field value if it's in the allow list. Also checking the case.
            var result = _allowedFields
                .FirstMatch(binder.Name)
                .DoIfSome(val => _dictionary[val] = value);

            return result.IsSome;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _dictionary.Keys;
        }

        public string TestProperty => "Test Method";
    }
}