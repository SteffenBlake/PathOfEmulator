namespace PathOfEmulator.API.Models
{
    public class ErrorResponse
    {
        public string Error { get; set; }
        public string Error_Description { get; set; }

        public static ErrorResponse InvalidToken()
        {
            return new ErrorResponse
            {
                Error = "invalid_token",
                Error_Description = "The access token provided is invalid"
            };
        }
    }
}
