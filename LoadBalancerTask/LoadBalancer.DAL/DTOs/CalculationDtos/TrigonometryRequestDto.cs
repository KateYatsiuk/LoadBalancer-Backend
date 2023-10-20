namespace LoadBalancer.DAL.DTOs.CalculationDtos
{
    public class TrigonometryRequestDto
    {
        public double? XForSin { get; set; }
        public double? XForCos { get; set; }
        public int N { get; set; }
        public string ConnectionId { get; set; }
    }
}
