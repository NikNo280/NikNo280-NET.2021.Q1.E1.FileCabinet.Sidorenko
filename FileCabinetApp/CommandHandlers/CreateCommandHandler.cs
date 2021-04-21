using System;
using FileCabinetApp.Validation.InputValidation;

namespace FileCabinetApp.CommandHandlers
{
     /// <summary>
     /// This class provides a create command hadler.
     /// </summary>
    public class CreateCommandHandler : ServiceCommandHandler
    {
        private readonly IInputValidation inputValidation;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        /// <param name="inputValidation">Input validator.</param>
        public CreateCommandHandler(IFileCabinetService fileCabinetService, IInputValidation inputValidation)
            : base(fileCabinetService)
        {
            this.inputValidation = inputValidation;
        }

        /// <summary>
        /// Command handler.
        /// </summary>
        /// <param name="appCommandRequest">Request.</param>
        public override void Handle(AppCommandRequest appCommandRequest)
        {
            if (appCommandRequest is null)
            {
                throw new ArgumentNullException($"{nameof(appCommandRequest)} is null");
            }

            if (string.Equals(appCommandRequest.Command, "create", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.Write("First name: ");
                var firstName = TypeConverter.ReadInput(TypeConverter.StringConverter, this.inputValidation.FirstNameValidator);
                Console.Write("Last  name: ");
                var lastName = TypeConverter.ReadInput(TypeConverter.StringConverter, this.inputValidation.SecondNameValidator);
                Console.Write("Date of birth: ");
                var dateOfBirth = TypeConverter.ReadInput(TypeConverter.DateTimeConverter, this.inputValidation.DateOfBirthValidator);
                Console.Write("Age: ");
                var age = TypeConverter.ReadInput(TypeConverter.ShortConverter, this.inputValidation.AgeValidator);
                Console.Write("Salary: ");
                var salary = TypeConverter.ReadInput(TypeConverter.DecimalConverter, this.inputValidation.SalaryValidator);
                Console.Write("Gender (M/W): ");
                var gender = TypeConverter.ReadInput(TypeConverter.CharConverter, this.inputValidation.GenderValidator);
                var record = new FileCabinetRecord
                {
                    Id = this.FileCabinetService.GetLastIndex() + 1,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    Age = age,
                    Salary = salary,
                    Gender = gender,
                };
                this.FileCabinetService.CreateRecord(record);
                Console.WriteLine($"Record #{this.FileCabinetService.GetLastIndex()} is created.");
            }
            else
            {
                if (this.NextCommandHandler != null)
                {
                    this.NextCommandHandler.Handle(appCommandRequest);
                }
                else
                {
                    throw new ArgumentException("Invalid command.");
                }
            }
        }
    }
}
