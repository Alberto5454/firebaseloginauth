using Microsoft.AspNetCore.Mvc;
using FireSharp.Config;
using FireSharp.Interfaces;
using crud_Informacion.Models;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace crud_Informacion.Controllers
{
    public class crudController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "DC3pURygxa74YRMXLC6b6sOe4kd0AEVtiL46WBD3",
            BasePath = "https://estudiante-data-base-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Estudiante estudiante)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = estudiante;
                PushResponse response = client.Push("Estudiante/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Estudiante/" + data.Id, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Agregado satisfactoriamente");

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Algo salio mal");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }
        public IActionResult List()
        {

            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Estudiante");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Estudiante>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Estudiante>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);

        }
        public ActionResult Delete(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Estudiante/" + id);
            return RedirectToAction("List");
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Estudiante/" + id);
            Estudiante data = JsonConvert.DeserializeObject<Estudiante>(response.Body);
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Estudiante estudiante)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Estudiante/" + estudiante.Estudiante_Id, estudiante);
            return RedirectToAction("List");
        }

    }
}
