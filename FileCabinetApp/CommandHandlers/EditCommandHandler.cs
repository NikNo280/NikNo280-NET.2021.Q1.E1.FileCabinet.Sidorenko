using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// This class provides a edit command hadler.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">File cabinet service.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
            : base(fileCabinetService)
        {
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

            if (string.Equals(appCommandRequest.Command, "edit", StringComparison.InvariantCultureIgnoreCase))
            {
                bool result;
                int id;
                result = int.TryParse(appCommandRequest.Parameters, out id);
                if (!result)
                {
                    Console.WriteLine("id is not a number");
                    return;
                }

                Console.Write("First name: ");
                var firstName = TypeConverter.ReadInput(TypeConverter.StringConverter, Program.recordValidator.NameValidator);
                Console.Write("Last  name: ");
                var lastName = TypeConverter.ReadInput(TypeConverter.StringConverter, Program.recordValidator.NameValidator);
                Console.Write("Date of birth: ");
                var dateOfBirth = TypeConverter.ReadInput(TypeConverter.DateTimeConverter, Program.recordValidator.DateOfBirthValidator);
                Console.Write("Age: ");
                var age = TypeConverter.ReadInput(TypeConverter.ShortConverter, Program.recordValidator.AgeValidator);
                Console.Write("Salary: ");
                var salary = TypeConverter.ReadInput(TypeConverter.DecimalConverter, Program.recordValidator.SalaryValidator);
                Console.Write("Gender (M/W): ");
                var gender = TypeConverter.ReadInput(TypeConverter.CharConverter, Program.recordValidator.GenderValidator);
                try
                {
                    var editRecord = new FileCabinetRecord
                    {
                        Id = id,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        Age = age,
                        Salary = salary,
                        Gender = gender,
                    };
                    this.FileCabinetService.EditRecord(editRecord);
                    Console.WriteLine($"Record #{id} is updated.");
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
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
