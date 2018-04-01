﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MELB_WS.Models.BBDD
{

    /*
         Definicion :  Clase que contiene la conexión hacia  la Base de datos.
    */

    public class ConexionBBDD
    {
        private string Cadena_Conexion = "Server=tcp:melb.database.windows.net,1433;Initial Catalog=MeLB;Persist Security Info=False;User ID=MeLbAdmin;Password=eXOsSMjv9;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public  SqlConnection Conexion;

        // Inicializa la conexión a la BBDD //
        public Boolean Abrir_Conexion_BBDD()
        {
            Conexion = new SqlConnection(Cadena_Conexion);
            try
            {
                Conexion.Open();
            }
            catch 
            {                
                return false;
            }
            return true;
        }

        // Cierra la conexión hacia la BBDD //
        public void Cerrar_Conexion()
        {
             Conexion.Dispose();
        }

    }
}