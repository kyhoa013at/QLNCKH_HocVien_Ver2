using System.ComponentModel.DataAnnotations;

namespace QLNCKH_HocVien.Helpers
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Validate model theo DataAnnotations và trả về danh sách lỗi
        /// </summary>
        public static (bool IsValid, List<string> Errors) ValidateModel<T>(T model) where T : class
        {
            var errors = new List<string>();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            if (!isValid)
            {
                errors.AddRange(results.Select(r => r.ErrorMessage ?? "Validation error"));
            }

            return (isValid, errors);
        }

        /// <summary>
        /// Kiểm tra chuỗi có null hoặc rỗng không
        /// </summary>
        public static bool IsNullOrEmpty(string? value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Kiểm tra ID hợp lệ (> 0)
        /// </summary>
        public static bool IsValidId(int? id) => id.HasValue && id.Value > 0;

        /// <summary>
        /// Kiểm tra điểm hợp lệ (0-10)
        /// </summary>
        public static bool IsValidScore(double score) => score >= 0 && score <= 10;
    }
}

