using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using MELB_WS.Models.BBDD;
using System.Data;
using MELB_WS.Models.Inventario.Modelos;
using Newtonsoft.Json;
using System.Web.Http;
using System.Net;

namespace MELB_WS.Models.Inventario.Operaciones
{
    /*
        Definicion :  Clase que contiene las operaciones CRUD
                      de los controladores que participan en
                      los procesos de inventario.
    */

    public class Operaciones_Inventario  : ApiController
    {
        private ConexionBBDD Instancia_BBDD;
        private SqlDataReader SqlReader;
        private SqlCommand CMD;
        private String Errores;
        private Dictionary<int, int> Diccionario_ID_Existe;
        private Dictionary<int, int> Diccionario_ID_No_Existe;

        // Inicializa la conexión hacia la BBDD //
        public Operaciones_Inventario()
        {
            Instancia_BBDD = new ConexionBBDD(); 
            Errores = "{\"Cod_Resultado\": -2,\"Errores\": {";
        }

        #region CRUD : Controlador Instrumento 
        // Devuelve la lista total de todos los instrumentos //
        public dynamic Devolver_Lista_Todos_Instrumentos(int Bandera = 1 , int ID_Instrumento = 0)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Listado_Instrumentos", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Instrumento", SqlDbType.Int).Value = ID_Instrumento;
                CMD.Parameters.Add("@Bandera", SqlDbType.Bit).Value = Bandera;
                SqlReader = CMD.ExecuteReader();
                List<Instrumento> Lista_Instrumento = new List<Instrumento>();
                if (SqlReader.HasRows)
                {
                    while (SqlReader.Read())
                    {
                        Instrumento Nuevo_Instrumento = new Instrumento();
                        Nuevo_Instrumento.ID_Instrumento = SqlReader.GetInt32(0);
                        Nuevo_Instrumento.Nombre = SqlReader.GetString(1);
                        Nuevo_Instrumento.Material = SqlReader.GetString(2);
                        Nuevo_Instrumento.Color = SqlReader.GetString(3);
                        Nuevo_Instrumento.Imagen = SqlReader.GetString(4);
                        Nuevo_Instrumento.Marca = SqlReader.GetString(5);
                        Nuevo_Instrumento.Descripcion = SqlReader.GetString(6);
                        Nuevo_Instrumento.Estado = SqlReader.GetString(7);
                        Nuevo_Instrumento.ID_Estuche = SqlReader.GetInt32(8);
                        Nuevo_Instrumento.Nombre_Estuche = SqlReader.GetString(9);
                        Nuevo_Instrumento.ID_Proveedor = SqlReader.GetInt32(10);
                        Nuevo_Instrumento.Proveedor = SqlReader.GetString(11);
                        Nuevo_Instrumento.Tipo_Ubicacion = SqlReader.GetString(16);
                        if (Nuevo_Instrumento.Tipo_Ubicacion == "Bodega")
                        {
                            Nuevo_Instrumento.Estante = SqlReader.GetInt32(12);
                            Nuevo_Instrumento.Gaveta = SqlReader.GetInt32(13);
                        }
                        else
                        {
                            Nuevo_Instrumento.Piso = SqlReader.GetInt32(15);
                            Nuevo_Instrumento.Numero_Aula = SqlReader.GetInt32(14);
                        }
                        Lista_Instrumento.Add(Nuevo_Instrumento);
                    }

                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return JsonConvert.SerializeObject(Lista_Instrumento, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return InternalServerError();
            }
        }

        // Inserta un instrumento dado su modelo //
        public dynamic Insertar_Instrumento(Instrumento Inst)
        {
            if(Inst.Tipo_Ubicacion == "1"){ Diccionario_ID_Existe = new Dictionary<int, int> { { 1, Inst.ID_Estuche }, { 3, Inst.ID_Proveedor } };}
            else { Diccionario_ID_Existe = new Dictionary<int, int> { { 1, Inst.ID_Estuche }, { 3, Inst.ID_Proveedor },{ 4,Convert.ToInt32(Inst.ID_Aula)}};}        
            Dictionary<int, int> Diccionario_ID_No_Existe = new Dictionary<int, int> {{ 2, Inst.ID_Instrumento }};
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                if (Validar_ID_Controlador_Inventario(Diccionario_ID_Existe,1) == 0 & Validar_ID_Controlador_Inventario(Diccionario_ID_No_Existe,0) == 0 )
                {
                    CMD = new SqlCommand("I_Insertar_Instrumento", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.StoredProcedure;
                    CMD.Parameters.Add("@V_ID_Instrumento", SqlDbType.Int).Value = Inst.ID_Instrumento;
                    CMD.Parameters.Add("@V_Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                    CMD.Parameters.Add("@V_Material", SqlDbType.VarChar).Value = Inst.Material;
                    CMD.Parameters.Add("@V_Color", SqlDbType.VarChar).Value = Inst.Color;
                    CMD.Parameters.Add("@V_Imagen", SqlDbType.VarChar).Value = Inst.Imagen;
                    CMD.Parameters.Add("@V_Marca", SqlDbType.VarChar).Value = Inst.Marca;
                    CMD.Parameters.Add("@V_Descripcion", SqlDbType.VarChar).Value = Inst.Descripcion;
                    CMD.Parameters.Add("@V_Esta_En_Bodega", SqlDbType.Bit).Value = Convert.ToInt32(Inst.Tipo_Ubicacion);
                    CMD.Parameters.Add("@V_Estado", SqlDbType.VarChar).Value = Inst.Estado;
                    CMD.Parameters.Add("@V_ID_Estuche", SqlDbType.Int).Value = Inst.ID_Estuche;
                    CMD.Parameters.Add("@V_ID_Proveedor", SqlDbType.Int).Value = Inst.ID_Proveedor;
                    if (Inst.Tipo_Ubicacion == "1") { CMD.Parameters.Add("@V_Estante ", SqlDbType.Int).Value = Inst.Estante; CMD.Parameters.Add("@V_Gaveta ", SqlDbType.Int).Value = Inst.Gaveta; }
                    else { CMD.Parameters.Add("@V_ID_Aula", SqlDbType.Int).Value = Inst.ID_Aula; }
                    CMD.ExecuteNonQuery();
                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return Content(HttpStatusCode.OK,"{\"Cod_Resultado\": 1,\"Mensaje\": \"Se inserto el nuevo registro\"}");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest,Errores + "}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }

        // Elimina un instrumento dado su identificador //
        public IHttpActionResult Eliminar_Instrumento (int Id)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Eliminar_Instrumento", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Instrumento", SqlDbType.Int).Value = Id;        
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se elimino el instrumento\"}");
            }
            else
            {
                return Content(HttpStatusCode.ServiceUnavailable, "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}");
            }
        }

        // Actualiza un instrumento dado su modelo //
        public IHttpActionResult Actualizar_Instrumento (Instrumento Inst)
        {
            if (Inst.Tipo_Ubicacion == "1") { Diccionario_ID_Existe = new Dictionary<int, int> { { 1, Inst.ID_Estuche }, { 3, Inst.ID_Proveedor }, { 2, Inst.ID_Instrumento }}; }
            else { Diccionario_ID_Existe = new Dictionary<int, int> { { 1, Inst.ID_Estuche }, { 3, Inst.ID_Proveedor }, { 4, Convert.ToInt32(Inst.ID_Aula) }, { 2, Inst.ID_Instrumento } }; }            
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                if (Validar_ID_Controlador_Inventario(Diccionario_ID_Existe, 1) == 0)
                {
                    CMD = new SqlCommand("I_Actualizar_Instrumento", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.StoredProcedure;
                    CMD.Parameters.Add("@V_ID_Instrumento", SqlDbType.Int).Value = Inst.ID_Instrumento;
                    CMD.Parameters.Add("@V_Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                    CMD.Parameters.Add("@V_Material", SqlDbType.VarChar).Value = Inst.Material;
                    CMD.Parameters.Add("@V_Color", SqlDbType.VarChar).Value = Inst.Color;
                    CMD.Parameters.Add("@V_Imagen", SqlDbType.VarChar).Value = Inst.Imagen;
                    CMD.Parameters.Add("@V_Marca", SqlDbType.VarChar).Value = Inst.Marca;
                    CMD.Parameters.Add("@V_Descripcion", SqlDbType.VarChar).Value = Inst.Descripcion;
                    CMD.Parameters.Add("@V_Esta_En_Bodega", SqlDbType.Bit).Value = Convert.ToInt32(Inst.Tipo_Ubicacion);
                    CMD.Parameters.Add("@V_Estado", SqlDbType.VarChar).Value = Inst.Estado;
                    CMD.Parameters.Add("@V_ID_Estuche", SqlDbType.Int).Value = Inst.ID_Estuche;
                    CMD.Parameters.Add("@V_ID_Proveedor", SqlDbType.Int).Value = Inst.ID_Proveedor;
                    if (Inst.Tipo_Ubicacion == "1") { CMD.Parameters.Add("@V_Estante ", SqlDbType.Int).Value = Inst.Estante; CMD.Parameters.Add("@V_Gaveta ", SqlDbType.Int).Value = Inst.Gaveta; }
                    else { CMD.Parameters.Add("@V_ID_Aula", SqlDbType.Int).Value = Inst.ID_Aula; }
                    CMD.ExecuteNonQuery();
                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se actualizo correctamente el registro\"}");                    
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest,Errores + "}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }
        #endregion


        #region CRUD : Controlador Proveedor 
        // Lista de todos los Proveedores e Individual //
        public dynamic Devolver_Lista_Todos_Proveedores(int Bandera = 1, int ID_Proveedor = 0)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Listado_Proveedores", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Proveedor", SqlDbType.Int).Value = ID_Proveedor;
                CMD.Parameters.Add("@Bandera", SqlDbType.Bit).Value = Bandera;
                SqlReader = CMD.ExecuteReader();
                List<Proveedor> Lista_Proveedor = new List<Proveedor>();
                if (SqlReader.HasRows)
                {
                    while (SqlReader.Read())
                    {
                        Proveedor Nuevo_Proveedor = new Proveedor();
                        Nuevo_Proveedor.ID_Proveedor = SqlReader.GetInt32(0);
                        Nuevo_Proveedor.Nombre = SqlReader.GetString(1);
                        Nuevo_Proveedor.Telefono_1 = SqlReader.GetDecimal(2);
                        Nuevo_Proveedor.Telefono_2 = SqlReader.GetDecimal(3);
                        Nuevo_Proveedor.Correo = SqlReader.GetString(4);
                        Nuevo_Proveedor.Direccion = SqlReader.GetString(5);
                        Nuevo_Proveedor.Imagen = SqlReader.GetString(6);
                        Lista_Proveedor.Add(Nuevo_Proveedor);
                    }

                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return JsonConvert.SerializeObject(Lista_Proveedor, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    return Content(HttpStatusCode.NotFound, "{\"Cod_Resultado\": 0,\"Mensaje\": \"La consulta no devolvio resultados\"}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }

        // Inserta un Proveedor dado su modelo //
        public IHttpActionResult Insertar_Proveedor(Proveedor Inst)
        {
            Diccionario_ID_No_Existe = new Dictionary<int, int> {{ 3, Inst.ID_Proveedor } };
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                if (Validar_ID_Controlador_Inventario(Diccionario_ID_No_Existe, 0) == 0)
                {
                    CMD = new SqlCommand("I_Insertar_Proveedor", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.StoredProcedure;
                    CMD.Parameters.Add("@ID_Proveedor", SqlDbType.Int).Value = Inst.ID_Proveedor;
                    CMD.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                    CMD.Parameters.Add("@Telefono_1", SqlDbType.Decimal).Value = Inst.Telefono_1;
                    CMD.Parameters.Add("@Telefono_2", SqlDbType.Decimal).Value = Inst.Telefono_2;
                    CMD.Parameters.Add("@Correo", SqlDbType.VarChar).Value = Inst.Correo;
                    CMD.Parameters.Add("@Direccion", SqlDbType.VarChar).Value = Inst.Direccion;
                    CMD.Parameters.Add("@Imagen", SqlDbType.VarChar).Value = Inst.Imagen;

                    CMD.ExecuteNonQuery();
                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se inserto el nuevo registro\"}");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, Errores + "}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }


        // Elimina un Proveedor dado su identificador //
        public IHttpActionResult Eliminar_Proveedor(int Id)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Eliminar_Proveedor", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Proveedor", SqlDbType.Int).Value = Id;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se elimino el instrumento\"}");
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }

        // Actualiza un Proveedor dado su modelo //
        public IHttpActionResult Actualizar_Proveedor(Proveedor Inst)
        {
            Diccionario_ID_Existe = new Dictionary<int, int> { { 3, Inst.ID_Proveedor } };
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                if (Validar_ID_Controlador_Inventario(Diccionario_ID_Existe, 1) == 0)
                {
                    CMD = new SqlCommand("I_Actualizar_Proveedor", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.StoredProcedure;
                    CMD.Parameters.Add("@ID_Proveedor", SqlDbType.Int).Value = Inst.ID_Proveedor;
                    CMD.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                    CMD.Parameters.Add("@Telefono_1", SqlDbType.Decimal).Value = Inst.Telefono_1;
                    CMD.Parameters.Add("@Telefono_2", SqlDbType.Decimal).Value = Inst.Telefono_2;
                    CMD.Parameters.Add("@Correo", SqlDbType.VarChar).Value = Inst.Correo;
                    CMD.Parameters.Add("@Direccion", SqlDbType.VarChar).Value = Inst.Direccion;
                    CMD.Parameters.Add("@Imagen", SqlDbType.VarChar).Value = Inst.Imagen;
                    CMD.ExecuteNonQuery();
                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return Content(HttpStatusCode.ServiceUnavailable,"{\"Cod_Resultado\": 1,\"Mensaje\": \"Se actualizo correctamente el registro\"}");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, Errores + "}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }
        #endregion


        #region CRUD : Controlador Estuche 
        // Devuelve la lista total de todos los Estuches //
        public dynamic Devolver_Lista_Todos_Estuches(int Bandera = 1, int ID_Estuche = 0)
        {        
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Listado_Estuches", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Estuche", SqlDbType.Int).Value = ID_Estuche;
                CMD.Parameters.Add("@Bandera", SqlDbType.Bit).Value = Bandera;
                SqlReader = CMD.ExecuteReader();
                List<Estuche> Lista_Estuche = new List<Estuche>();
                if (SqlReader.HasRows)
                {
                    while (SqlReader.Read())
                    {
                        Estuche Nuevo_Estuche = new Estuche();
                        Nuevo_Estuche.ID_Estuche = SqlReader.GetInt32(0);
                        Nuevo_Estuche.Nombre = SqlReader.GetString(1);
                        Nuevo_Estuche.Marca = SqlReader.GetString(2);
                        Nuevo_Estuche.Material = SqlReader.GetString(3);
                        Nuevo_Estuche.Color = SqlReader.GetString(4);
                        Nuevo_Estuche.Estado = SqlReader.GetString(5);

                        Lista_Estuche.Add(Nuevo_Estuche);
                    }

                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return JsonConvert.SerializeObject(Lista_Estuche, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    return Content(HttpStatusCode.NotFound,"{\"Cod_Resultado\": 0,\"Mensaje\": \"La consulta no devolvio resultados\"}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }

        // Inserta un Estuche dado su modelo //
        public IHttpActionResult Insertar_Estuche(Estuche Inst)
        {
            Diccionario_ID_No_Existe = new Dictionary<int, int> { { 1, Inst.ID_Estuche }};
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                if (Validar_ID_Controlador_Inventario(Diccionario_ID_No_Existe, 0) == 0)
                {

                    CMD = new SqlCommand("I_Insertar_Estuche", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.StoredProcedure;
                    CMD.Parameters.Add("@ID_Estuche", SqlDbType.Int).Value = Inst.ID_Estuche;
                    CMD.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                    CMD.Parameters.Add("@Marca", SqlDbType.VarChar).Value = Inst.Marca;
                    CMD.Parameters.Add("@Material", SqlDbType.VarChar).Value = Inst.Material;
                    CMD.Parameters.Add("@Color", SqlDbType.VarChar).Value = Inst.Color;
                    CMD.Parameters.Add("@Estado", SqlDbType.VarChar).Value = Inst.Estado;

                    CMD.ExecuteNonQuery();
                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se inserto el nuevo registro\"}");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, Errores + "}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }

        // Elimina un Estuche dado su identificador //
        public IHttpActionResult Eliminar_Estuche(int Id)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Eliminar_Estuche", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Estuche", SqlDbType.Int).Value = Id;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return Content(HttpStatusCode.OK ,"{\"Cod_Resultado\": 1,\"Mensaje\": \"Se elimino el Estuche\"}");
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }

        // Actualiza un Estuche dado su modelo //
        public IHttpActionResult Actualizar_Estuche(Estuche Inst)
        {
            Diccionario_ID_Existe = new Dictionary<int, int> { { 1, Inst.ID_Estuche } };
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                if (Validar_ID_Controlador_Inventario(Diccionario_ID_Existe, 1) == 0)
                {

                    CMD = new SqlCommand("I_Actualizar_Estuche", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.StoredProcedure;
                    CMD.Parameters.Add("@ID_Estuche", SqlDbType.Int).Value = Inst.ID_Estuche;
                    CMD.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                    CMD.Parameters.Add("@Marca", SqlDbType.VarChar).Value = Inst.Marca;
                    CMD.Parameters.Add("@Material", SqlDbType.VarChar).Value = Inst.Material;
                    CMD.Parameters.Add("@Color", SqlDbType.VarChar).Value = Inst.Color;
                    CMD.Parameters.Add("@Estado", SqlDbType.VarChar).Value = Inst.Estado;
                    CMD.ExecuteNonQuery();
                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return Content(HttpStatusCode.OK ,"{\"Cod_Resultado\": 1,\"Mensaje\": \"Se actualizo correctamente el registro\"}");
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, Errores + "}");
                }
            }
            else
            {
                return InternalServerError(new System.ArgumentException("{\"Exito\":\"false\",\"Mensaje_Cabecera\":\"Error\",\"Mensaje_Usuario\":\"Ocurrio un error en el servidor al procesar la petición\",\"Descripcion_Error\":\"No se puedo establecer una conexión con la Base de datos\"}", "Error"));
            }
        }
        #endregion


        #region CRUD : Controlador Accesorios 
        // Devuelve la lista total de todos los Estuches //
        public dynamic Devolver_Lista_Todos_Accesorios(int Bandera = 1, int ID_Accesorio = 0)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Listado_Accesorios", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Accesorio", SqlDbType.Int).Value = ID_Accesorio;
                CMD.Parameters.Add("@Bandera", SqlDbType.Bit).Value = Bandera;
                SqlReader = CMD.ExecuteReader();
                List<Accesorio> Lista_Accesorio = new List<Accesorio>();
                if (SqlReader.HasRows)
                {
                    while (SqlReader.Read())
                    {
                        Accesorio Nuevo_Accesorio = new Accesorio();
                        Nuevo_Accesorio.ID_Accesorio = SqlReader.GetInt32(0);
                        Nuevo_Accesorio.ID_Instrumento = SqlReader.GetInt32(1);
                        Nuevo_Accesorio.Nombre = SqlReader.GetString(2);
                        Nuevo_Accesorio.Descripcion = SqlReader.GetString(3);
                        
                        Lista_Accesorio.Add(Nuevo_Accesorio);
                    }

                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return JsonConvert.SerializeObject(Lista_Accesorio, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    return Content(HttpStatusCode.BadRequest, "{\"Cod_Resultado\": 0,\"Mensaje\": \"La consulta no devolvio resultados\"}");
                }
            }
            else
            {
                return Content(HttpStatusCode.ServiceUnavailable, "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}");
            }
        }

        // Inserta un Estuche dado su modelo //
        public IHttpActionResult Insertar_Accesorio(Accesorio Inst)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Insertar_Accesorio", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Accesorio", SqlDbType.Int).Value = Inst.ID_Accesorio;
                CMD.Parameters.Add("@ID_Instrumento", SqlDbType.Int).Value = Inst.ID_Instrumento;
                CMD.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                CMD.Parameters.Add("@Descripcion", SqlDbType.VarChar).Value = Inst.Descripcion;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se inserto el nuevo registro\"}");
            }
            else
            {
                return Content(HttpStatusCode.ServiceUnavailable,"{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}");
            }
        }

        // Elimina un Estuche dado su identificador //
        public IHttpActionResult Eliminar_Accesorio(int Id)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Eliminar_Accesorio", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Accesorio", SqlDbType.Int).Value = Id;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return Content(HttpStatusCode.OK, "{\"Cod_Resultado\": 1,\"Mensaje\": \"El Accesorio ha sido eliminado\"}");
            }
            else
            {
                return Content(HttpStatusCode.ServiceUnavailable, "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}");
            }
        }

        // Actualiza un Estuche dado su modelo //
        public IHttpActionResult Actualizar_Accesorio(Accesorio Inst)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("I_Actualizar_Accesorio", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Accesorio", SqlDbType.Int).Value = Inst.ID_Accesorio;
                CMD.Parameters.Add("@ID_Instrumento", SqlDbType.Int).Value = Inst.ID_Instrumento;
                CMD.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = Inst.Nombre;
                CMD.Parameters.Add("@Descripcion", SqlDbType.VarChar).Value = Inst.Descripcion;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return Content(HttpStatusCode.OK,"{\"Cod_Resultado\": 1,\"Mensaje\": \"Se actualizo correctamente el registro\"}");
            }
            else
            {
                return Content(HttpStatusCode.ServiceUnavailable ,"{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}");
            }
        }
        #endregion

        #region Validaciones: CRUD

        /* 
           Descripcion: Valida que los ID en BD no esten repetidos 
           Valores del diccionario:
               1 : Validacion ID_Estuche
               2 : Validacion ID_Instrumento
               3 : Validacion ID_Proveedor
               4 : Validacion ID_Aula
            Valores de la bandera
            0 : Generar nuevo tipo,
            1 : Validar que el tipo ya exista
        */
        public int Validar_ID_Controlador_Inventario(Dictionary<int, int> Lista_ID,int Bandera)
        {            
            int Contador_Errores = 0;
            foreach (KeyValuePair<int, int> Entrada in Lista_ID )
            {
                if(Entrada.Key == 1)
                {
                    CMD = new SqlCommand("SELECT DBO.Validar_ID_Estuche(@ID_Estuche)",Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.Text;
                    CMD.Parameters.Add(new SqlParameter("@ID_Estuche",Entrada.Value));
                    string Resultado = CMD.ExecuteScalar().ToString();
                    // Validando que el tipo ya exista //
                    if (Bandera == 1)
                    {
                        if ( Resultado == "0")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de estuche digitado no existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                    else
                    {
                        if (Resultado == "1")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de estuche digitado ya existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                }
                else if(Entrada.Key == 2)
                {
                    CMD = new SqlCommand("SELECT DBO.Validar_ID_Instrumento(@ID_Instrumento)", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.Text;
                    CMD.Parameters.Add(new SqlParameter("@ID_Instrumento", Entrada.Value));
                    string Resultado = CMD.ExecuteScalar().ToString();
                    // Validando que el tipo ya exista //
                    if (Bandera == 1)
                    {
                        if (Resultado == "0")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de instrumento digitado no existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                    else
                    {
                        if (Resultado == "1")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de instrumento digitado ya existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                }
                else if(Entrada.Key == 3)
                {
                    CMD = new SqlCommand("SELECT DBO.Validar_ID_Proveedor(@ID_Proveedor)", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.Text;
                    CMD.Parameters.Add(new SqlParameter("@ID_Proveedor", Entrada.Value));
                    string Resultado = CMD.ExecuteScalar().ToString();
                    // Validando que el tipo ya exista //
                    if (Bandera == 1)
                    {
                        if (Resultado == "0")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de proveedor digitado no existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                    else
                    {
                        if (Resultado == "1")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de proveedor digitado ya existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                }
                else if (Entrada.Key == 4)
                {
                    CMD = new SqlCommand("SELECT DBO.Validar_ID_Aula(@ID_Aula)", Instancia_BBDD.Conexion);
                    CMD.CommandType = CommandType.Text;
                    CMD.Parameters.Add(new SqlParameter("@ID_Aula", Entrada.Value));
                    string Resultado = CMD.ExecuteScalar().ToString();
                    // Validando que el tipo ya exista //
                    if (Bandera == 1)
                    {
                        if (Resultado == "0")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de aula digitado no existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                    else
                    {
                        if (Resultado == "1")
                        {
                            Errores = Errores + "[\"Mensaje\": \"El ID de aula digitado ya existe en la base de datos\"]";
                            Contador_Errores++;
                        }
                    }
                }
            }            
            return Contador_Errores;
        }
        #endregion
    }
}