using identity_server_anima.Domain.Enums;

namespace Identity.Utils
{
    public static class Converter
    {
        public static Role GetRole(string roleString)
        {
            if (Enum.TryParse<Role>(roleString, true, out Role meuEnumValue))
            {
                return meuEnumValue;
            }
            return Role.student;
        }
    }
}
