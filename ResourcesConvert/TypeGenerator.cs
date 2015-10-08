using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ResourcesConvert
{
    public static class TypeGenerator
    {
        public static dynamic CreateResourceObject(Type type)
        {
            return Activator.CreateInstance(type);
        }
        public static Type CreateResourceType(List<Property> list, string signature)
        {
            TypeBuilder tb = GetTypeBuilder(signature);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (Property property in list)
            {
                CreateProperty(tb, property.Name, property.Type);
            }
            Type objectType = tb.CreateType();
            return objectType;
        }
        private static TypeBuilder GetTypeBuilder(string signature)
        {
            AssemblyName assemblyName = new AssemblyName(signature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(signature, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout, null);
            return typeBuilder; 
        }
        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField(propertyName.ToUpper(), propertyType, FieldAttributes.Private);
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod("Get" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator GetIlGenerator = getMethodBuilder.GetILGenerator();

            GetIlGenerator.Emit(OpCodes.Ldarg_0);
            GetIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            GetIlGenerator.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod("Set" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
            ILGenerator setIlGenerator = setMethodBuilder.GetILGenerator();
            Label modifyProperty = setIlGenerator.DefineLabel();
            Label exitSet = setIlGenerator.DefineLabel();

            setIlGenerator.MarkLabel(modifyProperty);
            setIlGenerator.Emit(OpCodes.Ldarg_0);
            setIlGenerator.Emit(OpCodes.Ldarg_1);
            setIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);

            setIlGenerator.Emit(OpCodes.Nop);
            setIlGenerator.MarkLabel(exitSet);
            setIlGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
    public class Property
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public Property() { }
        public Property(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
