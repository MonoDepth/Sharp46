namespace Sharp46.Models
{
    public interface IFormRequest
    {
        public FormUrlEncodedContent ToFormEncoded();
    }
}
