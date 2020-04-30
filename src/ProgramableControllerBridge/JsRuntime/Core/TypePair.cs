using System;

namespace ProgramableControllerBridge.JsRuntime.Core {
    public class TypePair : IComparable<TypePair> {
        public Type WrapperType;
        public Type InnerType;

        public TypePair(Type wrapperType) {
            this.WrapperType = wrapperType;
            this.InnerType = wrapperType.BaseType.GetGenericArguments()[0];
        }

        public int CompareTo(TypePair that) {
            if (this.InnerType == that.InnerType) {
                return 0;
            } else if (this.InnerType.IsAssignableFrom(that.InnerType)) {
                return -1;
            } else if (that.InnerType.IsAssignableFrom(this.InnerType)) {
                return 1;
            } else {
                return 0;
            }
        }
    }
}