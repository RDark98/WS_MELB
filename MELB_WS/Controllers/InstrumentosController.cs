using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using MELB_WS.Models.Inventario.Operaciones;
using MELB_WS.Models.Inventario;
using System.Web.Http.Cors;

namespace MELB_WS.Controllers
{
    public class InstrumentosController : ApiController
    {
        Operaciones_Inventario Instancia_OP = new Operaciones_Inventario();
        
        // Retorno de toda la coleccion de datos //
        [SwaggerOperation("GetAll")]
        public dynamic Get()
        {
            return Instancia_OP.Devolver_Lista_Todos_Instrumentos();            
        }
        
        // Retorno de un registro de la coleccion de datos //
        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Get(int ID)
        {
            return Instancia_OP.Devolver_Lista_Todos_Instrumentos(0,ID);
        }

        // Creacion de un nuevo registro //
        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        [SwaggerResponse(HttpStatusCode.NotFound)]        
        public IHttpActionResult Post([FromBody]Instrumento Ins)
        {
            if (ModelState.IsValid && Ins != null)
            {
                return Instancia_OP.Insertar_Instrumento(Ins);
            }            
            else 
            {
                return BadRequest("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Asegurate de ingresar bien los datos\",\"Descripcion_Error\":\"El modelo enviado al servidor no es correcto\"}");           
            }
        }

        // Actualizacion de un registro ya existente //
        [SwaggerOperation("Update")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Put([FromBody]Instrumento Ins)
        {
            if (ModelState.IsValid && Ins != null)
            {
                return Instancia_OP.Actualizar_Instrumento(Ins);
            }
            else
            {
                return BadRequest("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Asegurate de ingresar bien los datos\",\"Descripcion_Error\":\"El modelo enviado al servidor no es correcto\"}");
            }
        }

        // Eliminación de un registro //
        [SwaggerOperation("Delete")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public IHttpActionResult Delete(int ID)
        {
            return Instancia_OP.Eliminar_Instrumento(ID);
        }
    }
}
