using EstructurasDatosPG3CS.ArbolesBinarios;
using System.Runtime.InteropServices;

namespace WS_ModeloTarjetasCS
{
    public class clsTarjetaCredito : clsComparador
    {
        public string lStrNumeroTarjeta { get; set; }
        public string lStrPropietarioTarjeta { get; set; }
        public string lStrFechaExpiracion { get; set; }
        public string lStrCodigoSeguridad { get; set; }
        public string lStrEstadoBloqueo { get; set; }
        public string lStrProveedorTarjeta { get; set; }
        public string lStrRedTarjeta { get; set; }
        public double lDblCreditoLimite { get; set; }
        public double lDblCreditoDisponible { get; set; }
        public string lStrFormaCobro { get; set; }
        public string lStrFechaCobro { get; set; }
        public double lDblTasaInteres { get; set; }
        public List<clsTransacciones> Transacciones { get; set; }
        //public clsPagos pagos { get; set; }

        public bool igualQue(object tarjeta)
        {
            clsTarjetaCredito lMiTarjeta = (clsTarjetaCredito)tarjeta;
            if (lStrNumeroTarjeta.CompareTo(lMiTarjeta.lStrNumeroTarjeta) == 0)
                return true;
            else
                return false;
        }

        public bool mayorIgualQue(object tarjeta)
        {
         clsTarjetaCredito lMiTarjeta = (clsTarjetaCredito)tarjeta;
         if (lStrNumeroTarjeta.CompareTo(lMiTarjeta.lStrNumeroTarjeta) >= 0)
            return true;
         else
            return false;
        }

        public bool mayorQue(object tarjeta)
        {
            clsTarjetaCredito lMiTarjeta = (clsTarjetaCredito)tarjeta;
            if (lStrNumeroTarjeta.CompareTo(lMiTarjeta.lStrNumeroTarjeta) > 0)
                return true;
            else
                return false;
        }

        public bool menorIgualQue(object tarjeta)
        {
            clsTarjetaCredito lMiTarjeta = (clsTarjetaCredito)tarjeta;
            if (lStrNumeroTarjeta.CompareTo(lMiTarjeta.lStrNumeroTarjeta) <= 0)
                return true;
            else
                return false;
        }

        public bool menorQue(object tarjeta)
        {
            clsTarjetaCredito lMiTarjeta = (clsTarjetaCredito)tarjeta;
            if (lStrNumeroTarjeta.CompareTo(lMiTarjeta.lStrNumeroTarjeta) < 0)
                return true;
            else
                return false;
        }
    }
}
