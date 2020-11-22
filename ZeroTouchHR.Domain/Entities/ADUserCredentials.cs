namespace ZeroTouchHR.Domain.Entities
{
    public class ADUserCredentials
    {

        //   string message = "dsadd user "cn=kishore,ou=users,ou=zerotouchhr,dc=zerotouchhr,dc=com" -fn Kishore -ln Poosa -pwd $ervice1@3 -email kishore3886@gmail.com -memberof cn=WorkSpaces,ou=zerotouchhr,dc=zerotouchhr,dc=com";

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        public string EmailAddress { get; set; }


        public override string ToString()
        {
            return $@"dsadd user cn={UserName},ou=users,ou=zerotouchhr,dc=zerotouchhr,dc=com -fn {FirstName} -ln {LastName}  -pwd {Password}  -email {EmailAddress} -memberof cn=WorkSpaces,ou=zerotouchhr,dc=zerotouchhr,dc=com";
        }
    }
}