using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using EstructurasDatosPG3CS.ListasSimples;
using Newtonsoft.Json;
using System.Collections.Generic;
using EstructurasDatosPG3CS.Colas;
using EstructurasDatosPG3CS.ArbolesBinarios;
using EstructurasDatosPG3CS.Pilas;
using System.Collections;

namespace WS_ModeloTarjetasCS.Controllers
{
    [ApiController]
    [Route("Tarjetas")]
    public class clsTarjetasController:ControllerBase
    {
        private readonly clsLSLista _listaSimple;
        private readonly clsCLista _ColaPago;
        private readonly clsArbolBinarioBusqueda _ArbolBinario;
        private readonly clsPLista _PilaTransacciones;

        public clsTarjetasController(clsLSLista listaSimple, clsCLista ColaPago, clsArbolBinarioBusqueda ArbolBinario, clsPLista PilaTransacciones)
        {
            _listaSimple = listaSimple;
            _ColaPago = _ColaPago;
            _ArbolBinario = ArbolBinario;
            _PilaTransacciones = PilaTransacciones;
        }

        //PUNTO 1 DEL PROYECTO
        [HttpGet]
        [Route("CargarInformacion")]
        public IActionResult LlenarListaSimple()
        {
            try
            {
                string jsonFilePath = "TarjetasCredito.json";
                string jsonString = System.IO.File.ReadAllText(jsonFilePath);
                List<clsTarjetaCredito> tarjetas = JsonConvert.DeserializeObject <List<clsTarjetaCredito>>(jsonString);

                foreach (var tarjeta in tarjetas)
                {
                    _listaSimple.InsertarInicio(tarjeta);
                }
                return Ok("Linked list filled with credit card information.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("MostrarLista")]
        public IActionResult ShowTarjetas()
        {
            if (_listaSimple.Primero != null)
            {
                try
                {
                    List<clsTarjetaCredito> creditCardsList = new List<clsTarjetaCredito>();

                    var currentNode = _listaSimple.Primero;
                    while (currentNode != null)
                    {
                        clsTarjetaCredito creditCard = (clsTarjetaCredito)currentNode.Elemento;
                        creditCardsList.Add(creditCard);
                        currentNode = currentNode.Siguiente;
                    }
                    return Ok(creditCardsList);
                }
                catch (Exception ex)
                {
                    return BadRequest($"An error occurred: {ex.Message}");
                }
            }
            else 
            {
                return NotFound("Linked list is empty.");
            }
        }

        //PUNTO 2 DEL PROYECTO
        [HttpPost]
        [Route("Balances")]
        public IActionResult ObtenerBalance(string NumeroTarjeta)
        {
            // VERIFICAMOS QUE LA LISTA NO ESTE VACIA
            if (_listaSimple.Primero != null)
            {
                var NodoActual = _listaSimple.Primero;
                while (NodoActual != null)
                {
                    clsTarjetaCredito MiTarjeta = (clsTarjetaCredito)NodoActual.Elemento;

                    if (string.Equals(NumeroTarjeta, MiTarjeta.lStrNumeroTarjeta))
                    {
                        decimal balance = Convert.ToDecimal(MiTarjeta.lDblCreditoLimite) - Convert.ToDecimal(MiTarjeta.lDblCreditoDisponible);

                        var Respuesta = new
                        {
                            PropietarioTarjeta = MiTarjeta.lStrPropietarioTarjeta,
                            NumeroTarjeta = MiTarjeta.lStrNumeroTarjeta,
                            Balance = balance
                        };
                        return Ok(Respuesta);
                    }
                    NodoActual = NodoActual.Siguiente;
                }
                return NotFound("TARJETA DE CREDITO NO ENCONTRADA.");
            }
            return NotFound("LISTA ESTA VACIA.");
        }

        //PUNTO 3 DEL PROYECTO
        [HttpPost]
        [Route("PagarBalance")]
        public IActionResult PagarTarjeta(string NumeroTarjeta, double CantidadPagar)
        {
            //VERIFICAMOS QUE LA LISTA NO ESTE VACIA
            if (_listaSimple.Primero != null)
            {
                var NodoActual = _listaSimple.Primero;
                while (NodoActual != null)
                {
                    clsTarjetaCredito MiTarjetaCredito = (clsTarjetaCredito)NodoActual.Elemento;

                    //BUSCAMOS LA TARJETA DE CREDITO INGRESADA
                    if (string.Equals(MiTarjetaCredito, MiTarjetaCredito.lStrNumeroTarjeta))
                    {
                        //CALCULAMOS EL NUEVO BALANCE
                        double NuevoBalance = MiTarjetaCredito.lDblCreditoDisponible - CantidadPagar;

                        //VERIFICAMOS QUE EL PAGO NO EXCEDA EL LIMITE 
                        if (NuevoBalance < 0)
                        {
                            return BadRequest("Payment amount exceeds the available balance.");
                        }

                        // ACTUALIZAMOS EL CREDITO DISPONIBLE
                        MiTarjetaCredito.lDblCreditoDisponible = NuevoBalance;

                        //RETORNAMOS EL BALANCE
                        //Y AGREGAMOS A LA COLA
                        var Respuesta = new
                        {
                            PropietarioTarjeta = MiTarjetaCredito.lStrPropietarioTarjeta,
                            NumeroTarjeta = MiTarjetaCredito.lStrNumeroTarjeta,
                            NuevoBalance = NuevoBalance
                        };
                        _ColaPago.Insertar(new clsPagos(Respuesta.NumeroTarjeta, DateTime.Now.ToString(), CantidadPagar));

                        return Ok($"SU PAGO DE: ${CantidadPagar} HA SIDO ACEPTADO.");
                    }

                    NodoActual = NodoActual.Siguiente;
                } // SI LA TARJETA NO FUE EONCONTRADA
                return NotFound("lA TARJETA NO FUE ENCONTRADA.");

            }//SI LA LISTA ESTA VACIA
            return NotFound("LA LISTA ESTA VACIA.");
        }

        [HttpGet]
        [Route("MostrarCola")]
        public IActionResult ShowPagos()
        {
            if (_ColaPago.ColaVacia != null)
            {
                try
                {
                    List<clsPagos> PagosListado = new List<clsPagos>();

                    var NodoActual = _ColaPago.Primero;
                    while (NodoActual != null)
                    {
                        clsPagos MiListado = (clsPagos)NodoActual.Elemento;
                        PagosListado.Add(MiListado);
                        NodoActual = NodoActual.Siguiente;
                    }
                    return Ok(PagosListado);
                }
                catch (Exception ex)
                {
                    return BadRequest($"UN ERROR A OCURRIDO: {ex.Message}");
                }
            }
            else
            {
                return NotFound("LISTA ESTA VACIA.");
            }
        }

        //PUNTO 4 DEL PROYECTO EN EL ARBOL QUEREMOS LA INFO SOLO DE LA PERSONA QUE ESTA PIDIENDO EL ESTADO DE CUENTA
        [HttpGet]
        [Route("InsertarEnArbol")]
        public IActionResult InsertarArbolDesdeLista(string NumeroTarjeta)
        {
            try
            {
                //METODO PARA INSERTAR AL ARBOL
                InsertarDesdeLista(_listaSimple, _ArbolBinario, NumeroTarjeta);

                return Ok("SE HA AGREGADO LA INFORMACION DE LISTA AL ARBOL.");
            }
            catch (Exception ex)
            {
                return BadRequest($"UN ERROR A OCURRIDO: {ex.Message}");
            }
        }

        //METODO PARA INSERTAR DE LA LISTA ENLAZADA A EL ARBOL BINARIO DE BUWSQUEDA
        private void InsertarDesdeLista(clsLSLista listaSimple, clsArbolBinarioBusqueda ArbolBinario, string NumeroTarjeta)
        {
            var NodoActual = listaSimple.Primero;
            while (NodoActual != null)
            {
                //NODO ACTUAL TRAE LA INFORMACION DE LA TARJETA INGRESADA
                clsTarjetaCredito MiTarjetaCredito = (clsTarjetaCredito)NodoActual.Elemento;

                //VERIFICAMOS QUE LA TARJETA ESTE EN LA LISTA Y EXISTA
                if (MiTarjetaCredito.lStrNumeroTarjeta == NumeroTarjeta)
                {
                    //ELIMINAMOS Y VOLVEMOS A INSERTAR EN EL ARBOL DE BUSQUEDA
                    ArbolBinario.eliminar(NumeroTarjeta);
                    ArbolBinario.insertar(NumeroTarjeta);
                    return; //SALIMONS DEL CICLO
                }

                NodoActual = NodoActual.Siguiente;
            }
        }

        [HttpPost]
        [Route("EstadoCuenta/{NumeroTarjeta}")]
        public IActionResult ObtenerBalanceCuenta(string NumeroTarjeta)
        {
            try
            {
                // ENCONTRAMOS LA TARJETA DE CREDITO
                clsABNodo MiNodoTarjetaCredito = _ArbolBinario.buscarIterativo(NumeroTarjeta);
                if (MiNodoTarjetaCredito != null)
                {
                    clsTarjetaCredito MiTarjetaCredito = (clsTarjetaCredito)MiNodoTarjetaCredito.valorNodo();
                    // CONSTRUIMOS EL BALANCE
                    string lStrBalance =       $"USUARIO DE TARJETA: {MiTarjetaCredito.lStrPropietarioTarjeta}\n" +
                                               $"NUMERO DE TARJETA: {MiTarjetaCredito.lStrNumeroTarjeta}\n" +
                                               $"LIMITE DE CREDITO: ${MiTarjetaCredito.lDblCreditoLimite}\n" +
                                               $"CREDITO DISPONIBLE: ${MiTarjetaCredito.lDblCreditoDisponible}";
                    return Ok(lStrBalance);
                }
                else
                {
                    return NotFound("LA TARJETA DE CREDITO NO FUE ENCONTRADA.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //PUNTO 5 DEL PROYECTO
        [HttpPost]
        [Route("InsertarEnPila/{NumeroTarjeta}")]
        public IActionResult InsertarTransaccionesPila(string NumeroTarjeta)
        {
            try
            {
                if (string.IsNullOrEmpty(NumeroTarjeta))
                {
                    return BadRequest("INGRESE LA TARJETA DE CREDITO.");
                }

                //BUSCANDO LA TARJETA DE CREDITO EN LA LISTA
                var NodoActual = _listaSimple.Primero;
                clsTarjetaCredito TarjetaCredito = null;
                while (NodoActual != null)
                {
                    clsTarjetaCredito MiTarjeta = (clsTarjetaCredito)NodoActual.Elemento;
                    if (string.Equals(NumeroTarjeta, MiTarjeta.lStrNumeroTarjeta))
                    {
                        TarjetaCredito = MiTarjeta;
                        break;
                    }
                    NodoActual = NodoActual.Siguiente;
                }

                if (TarjetaCredito == null)
                {
                    return NotFound("TARJETA DE CREDITO NO ENCONTRADA.");
                }

                // INSERTANDO LA TRANSACCION A LA PILA
                foreach (var Transacciones in TarjetaCredito.Transacciones)
                {
                    _PilaTransacciones.Insertar(Transacciones);
                }

                return Ok("LA TRANSACCION HA SIDO INSERTADA A LA PILA.");
            }
            catch (Exception ex)
            {
                return BadRequest($"UN ERROR A OCURRIDO: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Transacciones")]
        public IActionResult GetTransactions(string NumeroTarjeta)
        {
            try
            {
                // BUSCAMOS LAS TRANSACCIONES EN LA PILA
                var Transacciones = new List<string>();

                var NodoActual = _listaSimple.Primero;
                while (NodoActual != null)
                {
                    var MiTarjeta = (clsTarjetaCredito)NodoActual.Elemento;
                    if (MiTarjeta.lStrNumeroTarjeta == NumeroTarjeta)
                    {
                        // BUSCAMOS LAS TRANSACCIONES DE LA TARJETA Y LAS AÑADIMOS
                        foreach (var transaction in MiTarjeta.Transacciones)
                        {
                            Transacciones.Add(Transacciones.ToString());
                        }
                    }
                    NodoActual = NodoActual.Siguiente;
                }

                // VERIFICAMOS SI HAY TRANSACCIONES
                if (Transacciones.Any())
                {
                    return Ok(Transacciones);
                }
                else
                {
                    return NotFound("NO SE ENCONTRARON PARA LA TARJETA INGRESADA.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"UN ERROR A OCURRIDO: {ex.Message}");
            }
        }

        //PUNTO 7 DEL PROYECTO
        [HttpPost]
        [Route("CambiarCodigoSeguridad")]
        public IActionResult CambiarCodigoSeguridad(string NumeroTarjeta, string NuevoCodigoSeguridad)
        {
            try
            {
                // VERIFICAMOS SI LOS CAMPOS ESTAN LLENOS.
                if (string.IsNullOrEmpty(NumeroTarjeta) || string.IsNullOrEmpty(NuevoCodigoSeguridad))
                {
                    return BadRequest("INGRESE EL NUMERO DE LA TARJETA Y EL NUEVO CODIGO DE SEGURIDAD.");
                }

                // ENCONTRAR LA TARJETA DE CRÉDITO CON EL NÚMERO DE TARJETA PROPORCIONADO EN LA LISTA ENLAZADA
                var NodoActual = _listaSimple.Primero;
                while (NodoActual != null)
                {
                    var MiTarjeta = (clsTarjetaCredito)NodoActual.Elemento;
                    if (MiTarjeta.lStrNumeroTarjeta == NumeroTarjeta)
                    {
                        // ACTUALIZANDO EL CODIGO DE SEGURIDAD DE LA TARJETA CON EL MANDADO
                        MiTarjeta.lStrCodigoSeguridad = NuevoCodigoSeguridad;

                        // ACTUALIZAMOS EL REGISTRO EN LA LISTA SIMPLE
                        _listaSimple.InsertarInicio(MiTarjeta);

                        //CREAMOS UNA NUEVA LISTA SIMPLE E INSERTAMOS LA TARJETA CON EL CODIGO CAMBIADO
                        var newLinkedList = new clsLSLista();
                        newLinkedList.InsertarInicio(MiTarjeta);

                        return Ok("CODIGO DE SEGURIDAD CAMBIADO CORRECTAMENTE.");
                    }
                    NodoActual = NodoActual.Siguiente;
                }

                return NotFound("LA TARJETA DE CREDITO NO FUE ENCONTRADA.");
            }
            catch (Exception ex)
            {
                return BadRequest($"A OCURRIDO UN ERROR: {ex.Message}");
            }
        }
    }
}
