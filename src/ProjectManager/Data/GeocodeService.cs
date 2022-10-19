namespace ProjectManager.Data
{
    public class GeocodeService
    {
        HttpClient _client;
        HttpClient _externalClient;

        public GeocodeService(IHttpClientFactory factory)
        {
            _client = factory.CreateClient();
            _externalClient = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("subscription-key", "eVNTjXgQDABu083Xj-dKkuhuvHBggj06tJsaGABintY");
            _client.DefaultRequestHeaders.Add("x-ms-client-id", "b6489b08-04c7-48f6-94ef-b719551a5356");
        }

        public async Task<GeocodeAddress> GetAddress(string search)
        {
            string encodedSearch = Uri.EscapeDataString(search);
            GeocodeResponse response = await _client.GetFromJsonAsync<GeocodeResponse>($"https://atlas.microsoft.com/geocode?api-version=2022-02-01-preview&query={encodedSearch}");
            GeocodeFeature feature = response.Features.First();            

            CoordinateConversion coords = await _externalClient.GetFromJsonAsync<CoordinateConversion>($"https://webapps.bgs.ac.uk/data/webservices/CoordConvert_LL_BNG.cfc?method=LatLongToBNG&lat={feature.Geometry.Coordinates[1]}&lon={feature.Geometry.Coordinates[0]}");
            feature.Properties.Address.Easting = Math.Round(coords.Easting);
            feature.Properties.Address.Northing = Math.Round(coords.Northing);
            feature.Properties.Address.Latitude = feature.Geometry.Coordinates[1];
            feature.Properties.Address.Longitude = feature.Geometry.Coordinates[0];

            return feature.Properties.Address;
        }
    }

    public class GeocodeResponse
    {
        public List<GeocodeFeature> Features { get; set; }
    }

    public class GeocodeFeature
    {
        public string Type { get; set; }
        public GeocodeProperties Properties { get; set; }
        public GeocodeGeometry Geometry { get; set; }
    }

    public class GeocodeProperties
    {
        public GeocodeAddress Address { get; set; }
        public string Type { get; set; }
        public string Confidence { get; set; }
    }

    public class GeocodeAddress
    {
        public string FormattedAddress { get; set; }
        public string Locality { get; set; }
        public string PostalCode { get; set; }
        public string AddressLine { get; set; }

        public double Easting { get; set; }
        public double Northing { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class GeocodeGeometry
    {
        public string Type { get; set; }
        public double[] Coordinates { get; set; }
    }

    public class CoordinateConversion
    {
        public double Easting { get; set; }
        public double Northing { get; set; }
    }
}
