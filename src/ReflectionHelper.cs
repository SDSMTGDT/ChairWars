using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Xml;

namespace ChairWars
{
    public interface IXmlIO
    {
        XElement ToXML(string root = null);
        void FromXML(XElement obj);
    }

    public static class ReflectionHelper
    {
        public static bool ToXML<T>(this T obj, string fileName, string root) where T : IXmlIO
        {
            Debug.Assert(obj != null, String.Format("obj cannot be null. obj is {0}", obj.Name(_ => obj)));
            XDeclaration delc = new XDeclaration("1.0", "UTF-8", "Yes");
            XDocument doc = new XDocument(delc);
            doc.AddFirst(obj.ToXML(root));


            // doc.Save(fileName, SaveOptions.None);
            using (var writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                doc.Save(writer);
                return true;
            }

            //using (var writer = new TextWriter)
            //{
            //    // doc.WriteTo(writer);
            //    doc.Save(writer, SaveOptions.None);
            //    return true;
            //}
            return false;
        }

        public static bool FromXML<T>(this T obj, string fileName) where T : IXmlIO
        {
            Debug.Assert(obj != null, String.Format("obj cannot be null. obj is {0}", obj.Name(_ => obj)));

            // XDocument.Load(fileName, LoadOptions.None);

            // XPathDocument 
            using (var reader = new XmlTextReader(fileName))
            {
                var doc = XDocument.Load(reader, LoadOptions.None);
                obj.FromXML(doc.Root);
                return true;
            }
            return false;
        }

        private static string GetMemberName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    // var supername = GetMemberName(memberExpression.Expression);

                    // if (String.IsNullOrEmpty(supername))
                    return memberExpression.Member.Name;


                // return String.Concat(supername, '.', memberExpression.Member.Name);

                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    return callExpression.Method.Name;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return GetMemberName(unaryExpression.Operand);

                case ExpressionType.Parameter:
                    return String.Empty;

                default:
                    // throw new ArgumentException("The expression is not a member access or method call expression");
                    return "NA";
            }
        }

        public static XElement ToListXMLP<T>(this IEnumerable<T> src, Expression<Func<object, IEnumerable<T>>> expression) where T : struct
        {
            

            var Name = GetMemberName(expression.Body);
            if (null != src && src.Any())
            {
                XName item = "Item";
                return new XElement(Name, src.Select(f => new XElement(item, f)));
            }
            else
            {
                return new XElement(Name);
            }
        }

        public static XElement ToListXML<T>(this IEnumerable<T> src, Expression<Func<object, IEnumerable<T>>> expression) where T : IXmlIO
        {
            var Name = GetMemberName(expression.Body);
            if (null != src && src.Any())
            {

                return new XElement(Name, src.Select(f => f.ToXML("Item")));
            }
            else
            {
                return new XElement(Name);
            }
        }


        private static MemberExpression getMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return (MemberExpression)expression;

                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    return getMemberExpression(unaryExpression.Operand);

                //case ExpressionType.Parameter:
                //    var ExpressionParam = (ParameterExpression)expression;

                default:
                    // throw new ArgumentException("The expression is not a member access or method call expression");
                    return null;
            }
        }

        public static string Name<T>(this T obj, Expression<Func<T, object>> expression)
        {
            var exp = getMemberExpression(expression.Body);
            if (null == exp)
                return "NA";
            else
            {
                return exp.Member.Name;
            }
        }

        public static string TypeString<T>(this T obj, Expression<Func<T, object>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.MemberAccess)
            {
                var fieldInfo = (FieldInfo)((MemberExpression)expression.Body).Member;
                return fieldInfo.ReflectedType.ToString();
            }

            return string.Empty;
        }

        public static string classString<T>(this T obj, Expression<Func<T, object>> expression)
        {
            if (expression.Body.NodeType != ExpressionType.Constant)
            {
                return ((ConstantExpression)expression.Body).Type.ReflectedType.Name;
                // return fieldInfo.ReflectedType.ToString();
            }

            return string.Empty;
        }

        //public static bool SetValue<B,T>(this B obj, Expression<Func<T,object>> expression, XElement element)
        //{

        //    return true;
        //}

        //public static void ToXml<T>(this T obj, XElement elem) where T : class, new, IXmlIO
        //{
        //    if (obj == null)
        //        obj = new T();

        //    obj.FromXML(elem);
        //}




        public delegate V Func<T, U, V>(T input, out U output);


        //public static IEnumerable<T> SetValuesOrDefault<T>(this IEnumerable<T> obj,
        //      Expression<Func<T, object>> expression,
        //      XElement element,
        //      T def = default(T),
        //      Func<string, T, bool> func = null) where T : struct
        //{
        //    var list = obj.SetValues(expression, element, def, func);
        //    if (!list.Any())
        //        yield return def;
        //    else
        //        yield return list;

        //}

        public static IEnumerable<T> SetValues<T>(Expression<Func<object, IEnumerable<T>>> expression,
                                                  XElement element,
                                                  T def = default(T),
                                                  Func<string, T, bool> func = null) where T : struct
        {
            MemberExpression exp;
            if (null == (exp = getMemberExpression(expression.Body)))
                yield break;

            //var Root = obj.Name(_ => obj);

            // var fieldInfo = (FieldInfo)exp.Member;
            var Name = exp.Member.Name; //fieldInfo.Name;

            var values = element.Element(Name)
                                .Elements("Item");

            foreach (var elem in values.Where(e => e != null))
            {
                T t;
                if (Converter<T>(elem.Value, out t, def, func))
                    yield return t;
            }
            yield break;
        }


        public static IEnumerable<T> SetValues<T>(Expression<Func<object, IEnumerable<T>>> expression,
                                                  XElement element) where T : IXmlIO, new()
        {
            MemberExpression exp;
            if (null == (exp = getMemberExpression(expression.Body)))
                yield break;

            //var Root = obj.Name(_ => obj);

            //  Debug.Assert(Root == String.Empty || Root == null);

            // var fieldInfo = exp.Member as FieldInfo;
            // Debug.Assert(null != fieldInfo, "Must be a backing field, properties are not supported! (yet)");

            var Name = exp.Member.Name;//fieldInfo.Name;

            //Debug.Assert(Name == String.Empty || Name == null);

            var values = element.Element(Name)
                                .Elements("Item");

            if (null == values)
            {
                foreach (var elem in values)
                {
                    T t = new T();
                    t.FromXML(elem);
                    yield return t;
                }
            }
            yield break;
        }

        public static bool SetValue(Expression<Func<object, string>> expression,
                                       XElement elem, ref string member)
        {
            MemberExpression exp;
            if (null == (exp = getMemberExpression(expression.Body)))
                return false;

            //   var fieldInfo = exp.Member as FieldInfo;
            // Debug.Assert(null != fieldInfo, "Must be a backing field, properties are not supported! (yet)");
            var Name = exp.Member.Name; //fieldInfo.Name;

            //var prop = (PropertyInfo)exp.Member;
            //var Name = prop.Name;

            //Debug.Assert(Name == String.Empty || Name == null);


            if (!elem.HasElements)
                return false;

            var value = elem.Element(Name);
            if (value == null)
                return false;

            member = value.Value;
            return true;
        }

        public static bool SetValue<T>(Expression<Func<object, T>> expression,
                                       XElement element, ref T member) where T : class, IXmlIO, new()
        {
            MemberExpression exp;
            if (null == (exp = getMemberExpression(expression.Body)))
                return false;

            //   var fieldInfo = exp.Member as FieldInfo;
            //     Debug.Assert(null != fieldInfo, "Must be a backing field, properties are not supported! (yet)");
            var Name = exp.Member.Name; //fieldInfo.Name;

            //Debug.Assert(Name == String.Empty || Name == null, "Couldn't extract name");

            var value = element.Element(Name);
            if (null == value)
            {
                member = default(T);
                return false;
            }

            if (null == member)
            {
                member = new T();
            }

            member.FromXML(value);

            return true;
        }

        public static bool SetValue<T>(Expression<Func<object, T>> expression,
                                       XElement elem,
                                       ref T member,
                                       T def = default(T),
                                       Func<string, T, bool> func = null) where T : struct
        {
            member = def;
            MemberExpression exp;
            if (null == (exp = getMemberExpression(expression.Body)))
                return false;

            //            var fieldInfo = exp.Member as FieldInfo;
            //          Debug.Assert(null != fieldInfo, "Must be a backing field, properties are not supported! (yet)");
            var Name = exp.Member.Name;               //fieldInfo.Name;


            //var prop = (PropertyInfo)exp.Member;
            //var Name = prop.Name;

            //Debug.Assert(Name == String.Empty || Name == null);


            if (!elem.HasElements)
                return false;

            var value = elem.Element(Name);
            if (value == null)
                return false;

            //if (!prop.CanWrite)
            //    return false;

            return Converter(value.Value, out member, def, func);
        }


        private static bool Converter<T>(string strObj,
                                         out T result,
                                         T def = default(T),
                                         Func<string, T, bool> func = null) where T : struct
        {
            result = def;
            bool success = false;
            if (func == null)
            {
                MethodInfo convert;
                try
                {
                    // add caching mechanism. 
                    convert = result.GetType()
                                    .GetMethod("TryParse",
                                                  BindingFlags.Public | BindingFlags.Static);
                }
                catch
                {
                    result = def;
                    return false;
                }

                if (convert == null)
                {
                    result = def;
                    return false;
                }

                object[] param = new object[] { strObj, result };
                success = (bool)convert.Invoke(null, param);
            }
            else
            {
                success = func(strObj, out result);
            }

            if (!success)
            {
                result = def;
                return false;
            }
            return true;
        }



        // Useful function, but the convetions suck. Good reference though.

        //public static bool SetValue<T>(Expression<Func<object, T>> expression,
        //                               XElement elem,
        //                               ref T objInstance,
        //                               T def = default(T),
        //                               Func<string, T, bool> func = null) where T : struct
        //{
        //    MemberExpression exp;
        //    if(null == (exp = getMemberExpression(expression.Body)))
        //        return false;

        //    var Name = exp.Member.Name;
        //    var fieldInfo = (FieldInfo)exp.Member;
        //   // fieldInfo.ReflectedType.
        //  //  var prop = (PropertyInfo)exp.Member;
        //  //  Debug.Assert(Name == String.Empty || Name == null);

        //    if( !elem.HasElements )
        //        return false;

        //    var value = elem.Element(Name);
        //    if( value == null )
        //        return false;

        //    //if(!prop.CanWrite)
        //    //    return false;

        //    T result;
        //    Converter(value.Value, out result, def, func);
        //    try
        //    {
        //       // fieldInfo.SetValue(obj, result);
        //        fieldInfo.SetValue(obj, result);
        //    }
        //    catch
        //    {
        //        Debug.WriteLine("Failed to set the field.");
        //        return false;
        //    }

        //    return true;
        //}
    }

    /// EXAMPLE:
    ///     class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        class1 one = new class1
    //        {
    //            X = 1,
    //            Y = 3,
    //            Z = 44,
    //            Inner1 = new class2 { Data = 33, Data2 = 77 },
    //            Inner3 = new List<class2> { 
    //                                      new class2 { Data = 45, Data2 = 55 },
    //                                      new class2 { Data = 25, Data2 = 123 },
    //                                      new class2 { Data = 333, Data2 = 99 }
    //                                  },
    //            Inner4 = new double[] { 1.2, 2.3, 3.4, 4.5 }

    //        };

    //      // ReflectionHelper
    //       one.ToXML("test.xml", "One");

    //        class1 two = new class1();
    //        two.FromXML("test.xml");

    //        Console.ReadLine();
    //    }
    //}

    //public class class1 : IXmlIO
    //{
    //    private int x;




    //    public int X
    //    {
    //        get { return x; }
    //        set { x = value; }
    //    }
    //    private int y;

    //    public int Y
    //    {
    //        get { return y; }
    //        set { y = value; }
    //    }
    //    private int z;

    //    public int Z
    //    {
    //        get { return z; }
    //        set { z = value; }
    //    }
    //    private class2 inner1;

    //    public class2 Inner1
    //    {
    //        get { return inner1; }
    //        set { inner1 = value; }
    //    }
    //    //private class3 inner2;

    //    //public class3 Inner2
    //    //{
    //    //    get { return inner2; }
    //    //    set { inner2 = value; }
    //    //}
    //    private List<class2> inner3;

    //    public List<class2> Inner3
    //    {
    //        get { return inner3; }
    //        set { inner3 = value; }
    //    }

    //    private double[] inner4;

    //    public double[] Inner4
    //    {
    //        get { return inner4; }
    //        set { inner4 = value; }
    //    }

    //    public XElement ToXML(string root = null)
    //    {
    //        return new XElement(root,
    //                            inner1.ToXML(inner1.Name(_ => inner1)),
    //                            inner3.ToListXML(_ => inner3),
    //                            inner4.ToListXMLP(_ => inner4),
    //                            new XElement(this.Name(_ => x), x),
    //                            new XElement(this.Name(_ => y), y),
    //                            new XElement(this.Name(_ => z), z)
    //                           );
    //    }

    //    public void FromXML(XElement element)
    //    {
    //        ReflectionHelper.SetValue(_ => z, element, ref z, 0, int.TryParse);
    //        ReflectionHelper.SetValue(_ => x, element, ref x, 0, int.TryParse);
    //        ReflectionHelper.SetValue(_ => y, element, ref y, 0, int.TryParse);
    //        ReflectionHelper.SetValue(_ => inner1, element, ref inner1);
    //        inner3 = ReflectionHelper.SetValues(_ => inner3, element).ToList();
    //        inner4 = ReflectionHelper.SetValues(_ => inner4, element, 0.0, double.TryParse).ToArray();
    //    }
    //}

    //public class class2 : IXmlIO
    //{
    //    private int data;

    //    public int Data
    //    {
    //        get { return data; }
    //        set { data = value; }
    //    }
    //    private int data2;

    //    public int Data2
    //    {
    //        get { return data2; }
    //        set { data2 = value; }
    //    }
    //    //private class3 inner1c2;

    //    //public class3 Inner1c2
    //    //{
    //    //    get { return inner1c2; }
    //    //    set { inner1c2 = value; }
    //    //}

    //    public XElement ToXML(string root = null)
    //    {
    //        var rootName = String.IsNullOrEmpty(root) ? this.classString(_ => this) : root;
    //        return new XElement( rootName,
    //                             new XElement(this.Name(_ => data), data),
    //                             new XElement(this.Name(_ => data2), data2) // ,
    //                             );
    //    }

    //    public void FromXML(XElement element)
    //    {
    //        ReflectionHelper.SetValue(_ => data, element, ref data, 0, int.TryParse);
    //        ReflectionHelper.SetValue(_ => data2, element, ref data2, 0, int.TryParse);
    //    }
        
    //}

    //public class class3 : IXmlIO
    //{
    //    private string hello;

    //    public string Hello
    //    {
    //        get { return hello; }
    //        set { hello = value; }
    //    }

    //    public XElement ToXML(string root = null)
    //    {
    //        return new XElement( this.Name( _ => this ));
    //    }

    //    public void FromXML(XElement element)
    //    {

    //    }
    //}

    //public class class4 : IXmlIO
    //{
    //    private class3 inner1c4;

    //    public class3 Inner1c4
    //    {
    //        get { return inner1c4; }
    //        set { inner1c4 = value; }
    //    }

    //    public void FromXML(XElement element)
    //    {

    //    }

    //    public XElement ToXML(string root = null)
    //    {
    //        return new XElement("blah", "blah");
    //    }
    //}
}
