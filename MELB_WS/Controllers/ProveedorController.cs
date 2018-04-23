using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using MELB_WS.Models.Inventario.Operaciones;
using MELB_WS.Models.Inventario.Modelos;

namespace MELB_WS.Controllers
{
    public class ProveedorController : ApiController
    {
        Operaciones_Inventario Instancia_OP = new Operaciones_Inventario();

        // Retorno de toda la coleccion de datos //
        [SwaggerOperation("GetAll")]
        public dynamic Get()
        {
            return Instancia_OP.Devolver_Lista_Todos_Proveedores();
        }

        // Retorno de un registro de la coleccion de datos //
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public dynamic Get(int ID)
        {
            return Instancia_OP.Devolver_Lista_Todos_Proveedores(0,ID);
        }

        // Creacion de un nuevo registro //
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Post([FromBody]Proveedor Ins)
        {
            if (ModelState.IsValid && Ins != null)
            {
                return Instancia_OP.Insertar_Proveedor(Ins);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest,"{\"Cod_Resultado\": -1,\"Mensaje\": \"Asegurate de introducir correctamente todos los datos\"}");                
            }
        }

        // Actualizacion de un registro ya existente //
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Put([FromBody]Proveedor Ins)
        {
            if (ModelState.IsValid && Ins != null)
            {
                return Instancia_OP.Actualizar_Proveedor(Ins);
            }
            else
            {
                return Content(HttpStatusCode.BadRequest,"{\"Cod_Resultado\": -1,\"Mensaje\": \"Asegurate de introducir correctamente todos los datos\"}");                
            }

        }

        // Eliminación de un registro //
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Delete(int ID)
        {
            return Instancia_OP.Eliminar_Proveedor(ID);
        }

    }
}