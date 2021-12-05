using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HIPER.Helpers
{
    public interface IAuth
    {
        Task<bool> RegisterUser(string email, string password);
        Task<bool> LoginUser(string email, string password);
        bool IsAuhenticated();
        string GetCurrentUserId();
    };

    public class Auth
    {
        private static IAuth auth = DependencyService.Get<IAuth>();

        public static async Task<bool> RegisterUser(string email, string password)
        {
            try
            {
                return await Auth.RegisterUser(email, password);
            } catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                return false;
            }
            
        }

        public static async Task<bool> LoginUser(string email, string password)
        {
            try
            {
                return await auth.LoginUser(email, password);
            }catch(Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                string registerMessage = "There is no user record corresponding to this identifier.";
                if (ex.Message.Contains(registerMessage))
                    return await RegisterUser(email, password);
                return false;
            }
            
        }

        public static bool IsAuhenticated()
        {
            return Auth.IsAuhenticated();
        }

        public static string GetCurrentUserId()
        {
            return Auth.GetCurrentUserId();
        }


    }
}
