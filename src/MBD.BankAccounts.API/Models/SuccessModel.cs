using MeuBolsoDigital.Application.Utils.Responses.Interfaces;

namespace MBD.BankAccounts.API.Models
{
    public class SuccessModel<TData>
    {
        public string Message { get; set; }
        public TData Data { get; set; }

        public SuccessModel(IResult<TData> result)
        {
            Data = result.Data;
            Message = result.Message;
        }

        public SuccessModel() { }
    }
}