using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Web;
using System.Web.UI;
using GRA.SRP.Core.Utilities;
using GRA.SRP.DAL;
using SRPApp.Classes;
using GRA.Tools;
using System.Data;

namespace GRA.SRP.Controls {
    public partial class MyAvatarControl : System.Web.UI.UserControl {
        protected System.Web.Script.Serialization.JavaScriptSerializer json = new System.Web.Script.Serialization.JavaScriptSerializer();

        public Dictionary<string, List<int>> jsAvatarComponents = new Dictionary<string, List<int>>();


        protected void Page_Load(object sender, EventArgs e) {

            var patron = (Patron)Session["Patron"];

            var avatarPartData = AvatarPart.GetAll(patron.PID);

            foreach (DataRow data in avatarPartData.Tables[0].Rows)
            {
                Console.WriteLine(data["name"]);
            }

            var head = new List<int> { 1, 2, 7 };
            var body = new List<int> { 3, 4, 8 };
            var legs = new List<int> { 5, 6, 9 };

            jsAvatarComponents.Clear();
            jsAvatarComponents.Add("0", head);
            jsAvatarComponents.Add("1", body);
            jsAvatarComponents.Add("2", legs);


            if (!IsPostBack) {

                List<int> state = Patron.ReadAvatarStateString(patron.AvatarState); 

                if (state.Count == 3)
                {
                    componentState0.Value = state[0].ToString();
                    componentState1.Value = state[1].ToString();
                    componentState2.Value = state[2].ToString();
                }
            }
        }


        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        protected void Save()
        {
            var patron = (Patron)Session["Patron"];


            var avatarState = new List<int>();

            avatarState.Add(int.Parse(componentState0.Value));
            avatarState.Add(int.Parse(componentState1.Value));
            avatarState.Add(int.Parse(componentState2.Value));

            string state = Patron.WriteAvatarStateString(avatarState);
            patron.AvatarState = state;


            int width = 280;
            int height = 400;

            using (var bitmap = new Bitmap(width, height))
            {
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    foreach (var partID in avatarState)
                    {
                        var imageFile = $"/images/AvatarParts/{partID}.png";
                        var imageUrl = Server.MapPath(imageFile);

                        var image = Image.FromFile(imageUrl);

                        canvas.DrawImage(image, 0, 0);
                    }

                    canvas.Save();
                }
                try
                {
                    var oututUrl = Server.MapPath($"/images/AvatarCache/{patron.PID}.png");


                    bitmap.Save(oututUrl,
                                System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex) {

                }
            }
            patron.Update();
        }
  
    }
}