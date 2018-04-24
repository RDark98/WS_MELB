using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MELB_WS.Models.BBDD;
using System.Data;
using System.Web.Script.Serialization;
using MELB_WS.Models.PaNoJoderElResto.Modelos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MELB_WS.Models.PaNoJoderElResto.Operaciones
{

    /*
        Definicion :  Clase que contiene las operaciones CRUD
                      de los controladores que participan en
                      los procesos de inventario.
    */

    public class Operaciones
    {
        private ConexionBBDD Instancia_BBDD;
        private SqlDataReader SqlReader;
        private SqlCommand CMD;
        private String Errores;
        private Dictionary<int, int> Diccionario_ID_Existe;
        private Dictionary<int, int> Diccionario_ID_No_Existe;

        // Inicializa la conexión hacia la BBDD //
        public Operaciones()
        {
            Instancia_BBDD = new ConexionBBDD();
            Errores = "{\"Cod_Resultado\": -2,\"Errores\": {";
        }

        #region CRUD : Controlador Aulas 
        // Devuelve la lista total de todos las Aulas //
        public dynamic Devolver_Lista_Todos_Aulas(int Bandera = 1, int ID_Accesorio = 0)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("Listado_Aulas", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Aula", SqlDbType.Int).Value = ID_Accesorio;
                CMD.Parameters.Add("@Bandera", SqlDbType.Bit).Value = Bandera;
                SqlReader = CMD.ExecuteReader();
                List<Aula> Lista_Aula = new List<Aula>();
                if (SqlReader.HasRows)
                {
                    while (SqlReader.Read())
                    {
                        Aula Nuevo_Aula = new Aula();
                        Nuevo_Aula.ID_Aula = SqlReader.GetInt32(0);
                        Nuevo_Aula.Numero = SqlReader.GetInt32(1);
                        Nuevo_Aula.Piso = SqlReader.GetInt32(2);

                        Lista_Aula.Add(Nuevo_Aula);
                    }

                    CMD.Dispose();
                    Instancia_BBDD.Cerrar_Conexion();
                    return JsonConvert.SerializeObject(Lista_Aula, Formatting.None, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                }
                else
                {
                    return "{\"Cod_Resultado\": 0,\"Mensaje\": \"La consulta no devolvio resultados\"}";
                }
            }
            else
            {
                return "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}";
            }
        }

        // Inserta un Estuche dado su modelo //
        public string Insertar_Aula(Aula Inst)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("Insertar_Aula", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Aula", SqlDbType.Int).Value = Inst.ID_Aula;
                CMD.Parameters.Add("@Numero", SqlDbType.Int).Value = Inst.Numero;
                CMD.Parameters.Add("@Piso", SqlDbType.Int).Value = Inst.Piso;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se inserto el nuevo registro\"}";
            }
            else
            {
                return "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}";
            }
        }

        // Elimina un Estuche dado su identificador //
        public string Eliminar_Aula(int Id)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("Eliminar_Aula", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Aula", SqlDbType.Int).Value = Id;

                SqlParameter ResultadoDevuelto = new SqlParameter("@Resultado", DbType.Int32);
                ResultadoDevuelto.Direction = ParameterDirection.ReturnValue;

                CMD.Parameters.Add(ResultadoDevuelto);

                CMD.ExecuteNonQuery();

                int Resultado = Int32.Parse(CMD.Parameters["@Resultado"].Value.ToString());

                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();

                if (Resultado == 0)
                {
                    return "{\"Cod_Resultado\": 0,\"Mensaje\": \"El ID enviado no existe\"}";
                }
                else if (Resultado == -1)
                {
                    return "{\"Cod_Resultado\": 0,\"Mensaje\": \"El ID del Aula no se puede eliminar, porque esta en uso\"}";
                }
                else
                {
                    return "{\"Cod_Resultado\": 1,\"Mensaje\": \"El Aula ha sido eliminado\"}";
                }

                
            }
            else
            {
                return "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}";
            }
        }

        // Actualiza un Estuche dado su modelo //
        public string Actualizar_Aula(Aula Inst)
        {
            if (Instancia_BBDD.Abrir_Conexion_BBDD() == true)
            {
                CMD = new SqlCommand("Actualizar_Aula", Instancia_BBDD.Conexion);
                CMD.CommandType = CommandType.StoredProcedure;
                CMD.Parameters.Add("@ID_Aula", SqlDbType.Int).Value = Inst.ID_Aula;
                CMD.Parameters.Add("@Numero", SqlDbType.Int).Value = Inst.Numero;
                CMD.Parameters.Add("@Piso", SqlDbType.Int).Value = Inst.Piso;
                CMD.ExecuteNonQuery();
                CMD.Dispose();
                Instancia_BBDD.Cerrar_Conexion();
                return "{\"Cod_Resultado\": 1,\"Mensaje\": \"Se actualizo correctamente el registro\"}";
            }
            else
            {
                return "{\"Cod_Resultado\": -1,\"Mensaje\": \"No se pudo conectar con la base de datos\"}";
            }
        }
        #endregion
    }
}