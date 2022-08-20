using System.Collections.Generic;
using MeuBolsoDigital.Application.Utils.Responses.Interfaces;
using MeuBolsoDigital.CrossCutting.Extensions;

namespace MBD.BankAccounts.API.Models
{
    public class ErrorModel
    {
        public List<string> Errors { get; set; } = new List<string>();

        public ErrorModel(IResult result)
        {
            Errors.AddRange(result.Message.ConvertToArray());
        }

        public ErrorModel() { }
    }
}