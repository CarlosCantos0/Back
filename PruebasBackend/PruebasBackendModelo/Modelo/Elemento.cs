using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace PruebasBackendModelo.Modelo
{
    public partial class Elemento
    {
        public int Id { get; set; }
        public int IdEsquema { get; set; }
        public int IdTipoElemento { get; set; }
        public bool? Rellenado { get; set; }
        public double? CoordX { get; set; }
        public double? CoordY { get; set; }
        public double? Height { get; set; }
        public double? Width { get; set; }
        public string Stroke { get; set; }
        public string Fill { get; set; }
        public int? StrokeWidth { get; set; }
        public int? SizeLetter { get; set; }
        public string Text { get; set; }
        public string Fonts { get; set; }
        public double? X1 { get; set; }
        public double? X2 { get; set; }
        public double? Y1 { get; set; }
        public double? Y2 { get; set; }
        public string Name { get; set; }

        public double? ScaleX { get; set; }
        public double? ScaleY { get; set; }
        public double? Angle { get; set; }
        public string EstadoBorde { get; set; }

        public virtual Esquema IdEsquemaNavigation { get; set; }
        public virtual TipoElemento IdTipoElementoNavigation { get; set; }
    }
}
