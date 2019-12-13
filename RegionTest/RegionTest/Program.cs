namespace RegionTest
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Text;
    using DotSpatial.Data;
    using DotSpatial.Topology;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            do
            {
                Console.WriteLine("Enter coords");
                double x = 23;
                double y = 53;

                Double.TryParse(Console.ReadLine(), out x);
                Double.TryParse(Console.ReadLine(), out y);

                Stopwatch stopwatch = new Stopwatch();

                // Begin timing.
                stopwatch.Start();

                Feature f0 = PointInShapeRegion(@".\Data\Województwa.shp", x, y);
                Print(f0);
                Feature f1 = PointInShapeRegion(@".\Data\Powiaty.shp", x, y);
                Print(f1);
                Feature f2 = PointInShapeRegion(@".\Data\Gminy.shp", x, y);
                Print(f2);

                // Stop timing.
                stopwatch.Stop();

                // Write result.
                Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
                Console.WriteLine("Press esc to exit or other key to continue");
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
        }
        //https://pl.m.wikipedia.org/wiki/TERC  
        //http://www.gugik.gov.pl/pzgik/dane-bez-oplat/dane-z-panstwowego-rejestru-granic-i-powierzchni-jednostek-podzialow-terytorialnych-kraju-prg
        //https://gis-support.pl/granice-administracyjne/

        public static Feature PointInShapeRegion(string filename, double x, double y)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Load a shapefile.  If the shapefile is already using your custom projection, we don't need to change it.
            Shapefile wierdShapefile = Shapefile.OpenFile(filename);

            // Note, if your shapefile with custom projection has a .prj file, then we don't need to mess with defining the projection.
            // If not, we can define the projection as follows:
            // First get a ProjectionInfo class for the normal UTM projection
            //ProjectionInfo pInfo = DotSpatial.Projections.KnownCoordinateSystems.Projected.UtmNad1983.NAD1983UTMZone10N;

            //// Next modify the pINfo with your custom False Northing
            //pInfo.FalseNorthing = 400000;

            //wierdShapefile.Projection = pInfo;

            //// Reproject the strange shapefile so that it is in latitude/longitude coordinates
            //wierdShapefile.Reproject(DotSpatial.Projections.KnownCoordinateSystems.Geographic.World.WGS1984);

            // Define the WGS84 Lat Lon point to test
            Coordinate test = new Coordinate(x, y);
UInt64 i = 0;
            foreach (Feature f in wierdShapefile.Features)
            {
                UInt64 j = 0;


                foreach (var c in f.Coordinates)
                {
                    ++i;
                    ++j;
                //    Console.WriteLine($"x={c.X} y={c.Y} z={c.Z} m={c.M} {c.NumOrdinates}");
                }
                Console.WriteLine($"j={j}");


                Polygon pg = f.BasicGeometry as Polygon;
                if (pg != null)
                {
                    if (pg.Contains(new Point(test)))
                    {
                        // If the point is inside one of the polygon features
                        return f;
                    }
                }
                else
                {
                    // If you have a multi-part polygon then this should also handle holes I think
                    MultiPolygon polygons = f.BasicGeometry as MultiPolygon;
                    if (polygons.Contains(new Point(test)))
                    {
                        return f;
                    }
                }
            }
                Console.WriteLine($"i={i}");

            return null;
        }

        public static void Print(Feature f)
        {
            if (f == null)
                return;
            foreach (var item in f.DataRow.ItemArray)
                Console.WriteLine(item.ToString());

            DataTable table = f.DataRow.Table;

            Console.WriteLine(table.Rows.Count);

            string s = DumpDataTable(table);
            Console.WriteLine(s);
        }

        public static string DumpDataTable(DataTable table)
        {
            string data = string.Empty;
            StringBuilder sb = new StringBuilder();

            if (null != table && null != table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    sb.Append(column);
                    sb.Append(',');
                }

                sb.AppendLine();

                foreach (DataRow dataRow in table.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        sb.Append(item);
                        sb.Append(',');
                    }
                    sb.AppendLine();
                }

                data = sb.ToString();
            }
            return data;
        }
    }
}
