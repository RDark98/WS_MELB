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
    public class AccesorioController : ApiController
    {
        Operaciones_Inventario Instancia_OP = new Operaciones_Inventario();

        // Retorno de toda la coleccion de datos //
        [SwaggerOperation("GetAll")]
        public string Get()
        {
            return Instancia_OP.Devolver_Lista_Todos_Accesorios();
        }

        // Retorno de un registro de la coleccion de datos //
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Get(int ID)
        {
            return Instancia_OP.Devolver_Lista_Todos_Accesorios(0, ID);
        }

        // Creacion de un nuevo registro //
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Post([FromBody]Accesorio Ins)
        {
            if (ModelState.IsValid && Ins != null)
            {
                return Instancia_OP.Insertar_Accesorio(Ins);
            }
            else
            {
                return "{\"Cod_Resultado\": -1,\"Mensaje\": \"Asegurate de introducir correctamente todos los datos\"}";                
            }
        }

        // Actualizacion de un registro ya existente //
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Put([FromBody]Accesorio Ins)
        {
            if (ModelState.IsValid && Ins != null)
            {
                return Instancia_OP.Actualizar_Accesorio(Ins);
            }
            else
            {
                return "{\"Cod_Resultado\": -1,\"Mensaje\": \"Asegurate de introducir correctamente todos los datos\"}";                
            }
        }

        // Eliminación de un registro //
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public string Delete(int ID)
        {
            return Instancia_OP.Eliminar_Accesorio(ID);
        }

    }
}