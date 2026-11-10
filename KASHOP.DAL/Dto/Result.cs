using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; }

        public List<string>? Errors { get; set; }
        public static Result<T> Ok( string message = "Success")
        {
            return new Result<T> { Success = true, Message = message };
        }

        public static Result<T> Ok(T data, string message = "Success")
        {
            return new Result<T> { Success = true, Data = data, Message = message };
        }
        public static Result<T> Fail(string message)
        {
            return new Result<T> { Success = false, Message = message };
        }


        public static Result<T> Fail(List<string> errors, string message)
        {
            return new Result<T> { Success = false, Errors = errors, Message = message };
        }
    }
    
    }

