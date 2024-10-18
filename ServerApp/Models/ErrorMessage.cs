using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerApp.Models
{
    /// <summary>
    /// ErrorMessage Class for logging & displaying error
    /// </summary>
    public class ErrorMessage
    {
        public Guid Id { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Constructor initiallizing values
        /// </summary>
        /// <param name="statusCode">Status Code attached to the error</param>
        /// <param name="message">Message explaining Error</param>
        public ErrorMessage(int statusCode, string message)
        {
            Id = Guid.NewGuid();
            StatusCode = statusCode;
            Message = message;
        }

        /// <summary>
        /// To return back errorMessage details in JSON format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
