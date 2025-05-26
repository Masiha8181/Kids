using Newtonsoft.Json;
using Kids.Models;
using Kids.DTO;
using System.Net.Http;
using System.Numerics;
using System.Text;

namespace Kids.Services
{
    public class SmsService
    {
        public string ResultMessege { get; set; }
        public bool Result { get; set; }

        public SmsService()
        {
            Result = false;
            ResultMessege = "خطایی رخ داده است.";
        }




        public static async Task<SmsService> SendSmsAsync(string PhoneRecive, string token)
        {
            var smsService = new SmsService();
            var apiKey = "4A7A54465976617538696331356C38386E6A4B50675631694E2B714D7A794344564C784A47422F7967756F3D";
            var url = $"https://api.kavenegar.com/v1/{apiKey}/verify/lookup.json?receptor={PhoneRecive}&token={token}&template=NFAcademy";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

                        if (result != null && result.@return != null && result.@return.status == 200)
                        {
                            smsService.Result = true;
                            smsService.ResultMessege = "پیامک با موفقیت ارسال شد.";
                        }
                        else
                        {
                            smsService.Result = false;
                            smsService.ResultMessege = $"خطا در ارسال پیامک. پاسخ سرور: {responseBody}";
                        }
                    }
                    else
                    {
                        smsService.Result = false;
                        smsService.ResultMessege = $"خطای HTTP: {response.StatusCode}";
                    }
                }
            }
            catch (Exception e)
            {
                smsService.Result = false;
                smsService.ResultMessege = "خطا در ارسال پیامک: " + e.Message;
            }

            return smsService;
        }


    }
}

        
