using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER.Helpers
{
    public interface IFirestore
    {
        bool InsertUser(UserModel user);
        Task<bool> DeleteUser(UserModel user);
        Task<bool> UpdateUser(UserModel user);
        Task<List<UserModel>> ReadUser();
    }

    public class Firestore
    {
        private static IFirestore firestore = DependencyService.Get<IFirestore>();

        public static bool InsertUser(UserModel user)
        {
            return firestore.InsertUser(user);
        }

        public static async Task<bool> UpdateUser(UserModel user)
        {
            return await firestore.UpdateUser(user);
        }

        public static async Task<bool> DeleteUser(UserModel user)
        {
            return await firestore.DeleteUser(user);
        }

        public static async Task<List<UserModel>> ReadUser(UserModel user)
        {
            return await firestore.ReadUser();
        }
    }
}
