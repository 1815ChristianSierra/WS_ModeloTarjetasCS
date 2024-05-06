namespace WS_ModeloTarjetasCS
{
    public class clsPagos
    {
        public string lStrNumeroTarjeta { get; set; }
        public string lStrFecha { get; set; }
        public double lDblMonto { get; set; }

        public clsPagos(string vlstrNumeroTarjeta, string vlstrFecha, double vlDblMonto) 
        {
            this.lStrNumeroTarjeta = vlstrNumeroTarjeta;
            this.lStrFecha = vlstrFecha;
            this.lDblMonto = vlDblMonto;
        }
    }
}
