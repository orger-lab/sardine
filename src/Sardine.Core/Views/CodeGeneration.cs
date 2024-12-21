using System.Reflection;
using System.Reflection.Emit;

namespace Sardine.Core.Views
{
    internal static class CodeGeneration
    {
        internal static void CreateConstructor(TypeBuilder typeBuilder, Type baseVesselType)
        {
            ConstructorBuilder cBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                CallingConventions.HasThis,
                [typeof(Vessel), typeof(string[]), typeof(bool), typeof(int), typeof(List<IVesselPropertyToEventLink>)]);

            ILGenerator ilGen = cBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Ldarg_2);
            ilGen.Emit(OpCodes.Ldarg_3);
            ilGen.Emit(OpCodes.Ldarg_S, 4);
            ilGen.Emit(OpCodes.Ldarg_S, 5);
            
            ilGen.Emit(OpCodes.Call, typeof(VesselViewModel<>).MakeGenericType(baseVesselType)
                                                              .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
                                                                              [
                                                                                  typeof(Vessel<>).MakeGenericType(baseVesselType),
                                                                                  typeof(string[]),
                                                                                  typeof(bool),
                                                                                  typeof(int),
                                                                                  typeof(IList<IVesselPropertyToEventLink>)
                                                                              ])!);
            
            ilGen.Emit(OpCodes.Nop);
            ilGen.Emit(OpCodes.Nop);

            ilGen.Emit(OpCodes.Ret);
        }

        internal static void CreateProperty(TypeBuilder tb,
                                            string propertyName,
                                            Type propertyType,
                                            bool createSetter,
                                            bool createGetter,
                                            Type baseVesselType,
                                            FieldInfo vesselHandleField,
                                            MethodInfo vesselHandleGetMethod)
        {

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.None, propertyType, null);

            if (createGetter)
            {
                MethodBuilder getPropMthdBldr = tb.DefineMethod(
                    "get_" + propertyName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    propertyType,
                    Type.EmptyTypes);

                ILGenerator getIl = getPropMthdBldr.GetILGenerator();


                getIl.Emit(OpCodes.Ldarg_0);
                getIl.Emit(OpCodes.Ldfld, vesselHandleField);
                getIl.Emit(OpCodes.Callvirt, vesselHandleGetMethod); //get handle from vessel
                getIl.Emit(OpCodes.Callvirt, baseVesselType.GetProperty(propertyName)!.GetMethod!); //get value from handle
                getIl.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getPropMthdBldr);
            }

            if (createSetter)
            {
                MethodBuilder setPropMthdBldr =
                    tb.DefineMethod("set_" + propertyName,
                      MethodAttributes.Public |
                      MethodAttributes.SpecialName |
                      MethodAttributes.HideBySig,
                      null, [propertyType]);

                ILGenerator setIl = setPropMthdBldr.GetILGenerator();

                MethodInfo propChangedMethod = tb.BaseType!.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)])!;

                setIl.Emit(OpCodes.Nop);
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldfld, vesselHandleField);
                setIl.Emit(OpCodes.Callvirt, vesselHandleGetMethod); //get handle from vessel
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Callvirt, baseVesselType.GetProperty(propertyName)!.SetMethod!); //gsetvalue from handle

                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Ldstr, propertyName);
                setIl.Emit(OpCodes.Callvirt, propChangedMethod);
                setIl.Emit(OpCodes.Nop);


                setIl.Emit(OpCodes.Ret);

                propertyBuilder.SetSetMethod(setPropMthdBldr);
            }
        }

        // https://stackoverflow.com/questions/3862226/how-to-dynamically-create-a-class
        internal static void CreatePropertyForValueTypeVessel(TypeBuilder tb, Type vesselType, FieldInfo vesselHandleField, MethodInfo vesselHandleGetMethod)
        {
            PropertyBuilder propertyBuilder = tb.DefineProperty("Value", PropertyAttributes.None, vesselType, null);

            MethodBuilder getPropMthdBldr = tb.DefineMethod(
                "get_Value",
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                vesselType,
                Type.EmptyTypes);

            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, vesselHandleField);
            getIl.Emit(OpCodes.Callvirt, vesselHandleGetMethod); //get handle from vessel
            getIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
        }
    }
}