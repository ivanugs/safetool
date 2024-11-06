# Safetool - Risk Analysis Management System

![Safetool Logo](path/to/logo.png) <!-- Optional: Add project logo here -->

Safetool is an ASP.NET Core application designed to help companies manage risk analysis records for equipment. This application ensures that operators are aware of associated risks by confirming that they have read the necessary safety information. It offers features for both operators and administrators, streamlining safety compliance and record-keeping.

## Features

### For Operators
- **Risk Analysis Acknowledgment**: Allows operators to confirm they have read the risk analysis for each piece of equipment they handle.
- **Equipment List with Status**: View equipment with clear visual status indicators, including a green checkmark for already acknowledged equipment.
- **Automated Notifications**: Receive email notifications when acknowledgment records expire, prompting re-registration for continued compliance.

### For Administrators
- **Equipment Management**: Add, edit, and remove equipment, locations, and areas to keep master data up-to-date.
- **Role-Based Access Control**: Assign roles to users to maintain structured permissions, with administrators able to manage data while operators confirm acknowledgments.
- **Compliance Tracking**: View acknowledgment history, track expired acknowledgments, and monitor compliance for audit and reporting purposes.

## Project Structure

- **Controllers**: Handle the business logic for equipment and acknowledgment management.
- **Models**: Define the data structure for `Equipment`, `Locations`, `Areas`, and `User` roles.
- **Views**: User interface with role-specific layouts, offering easy navigation for both administrators and operators.
- **Database**: Stores all relevant information, including equipment details, user acknowledgments, and expiration dates.

## Requirements

- **User Roles**: Safetool uses two primary roles:
  - **Operators**: Users who confirm they have read risk analyses.
  - **Administrators**: Users who manage equipment, locations, areas, and assign roles.
  
- **Notification System**: Safetool automatically sends email notifications to operators for expired records, reminding them to re-register their acknowledgment.

## Installation

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/safetool.git
   cd safetool
   ```

2. **Configure the Database**: Update the database connection string in `appsettings.json`.

3. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**:
   ```bash
   dotnet run
   ```

## Usage

1. **Logging In**:
   - Admins can log in to manage equipment, users, and locations.
   - Operators can log in to view and acknowledge equipment risk analyses.

2. **Acknowledging Risk Analyses**:
   - Navigate to the Equipment List.
   - Acknowledge equipment with unread analyses by clicking the acknowledgment button.

3. **Tracking Compliance**:
   - Admins can monitor which equipment has been acknowledged and identify expired records.

## Notifications

- Safetool performs a daily review of all acknowledgment records.
- Records that exceed the defined expiration period will automatically trigger an **expired registration notification** email to the operator.

## Screenshots

| Operator Dashboard                        | Admin Equipment Management               |
| ----------------------------------------- | ---------------------------------------- |
| ![Operator Dashboard](path/to/screenshot) | ![Admin Dashboard](path/to/screenshot)   |

## Contributing

1. Fork the repository
2. Create a new branch (`feature/YourFeature`)
3. Commit your changes
4. Push to the branch
5. Open a pull request

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---

With Safetool, companies can enhance workplace safety by ensuring employees stay informed about the risks associated with their equipment.
