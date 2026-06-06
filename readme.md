# Password Reset Simulator

**Course Project:** Password Reset Application using Brute Force and Multi-Threading
**Student:** Aleem Shah
**Project Start Date:** 02 June 2026

## Project Description

This application simulates a password recovery process using a brute force attack. The program generates a random password, hashes it using SHA256 with a static salt, and attempts to recover it through brute force techniques. The final application will include both single-threaded and multi-threaded brute force implementations, performance comparison, and a graphical user interface built using Avalonia UI.

---

# Development Log

## Version 1.0 – 02 June 2026

### Completed Tasks

#### Project Setup

* Created Avalonia MVVM project.
* Configured project folder structure.
* Verified successful compilation and execution.

#### Password Hashing

* Implemented `PasswordHasher.cs`.
* Added SHA256 hashing functionality.
* Added constant static salt:

  * `COMP123_STATIC_SALT`

#### Password Generation

* Implemented `PasswordManager.cs`.
* Added random password generation.
* Password length generated between 4 and 5 characters (`[4–6)` requirement).

#### Password Validation

* Implemented `PasswordValidator.cs`.
* Added functionality to compare generated hashes against candidate passwords.

#### Brute Force Generator

* Implemented `BruteForceGenerator.cs`.
* Added recursive generation of all possible character combinations.
* Supports password lengths from 1 up to a specified maximum length.

#### Initial User Interface Test

* Connected backend classes to Avalonia UI.
* Displayed:

  * Generated password
  * Generated SHA256 hash
* Verified successful execution of hashing and password generation logic.

### Current Project Structure

* Models

  * PasswordManager.cs

* Security

  * PasswordHasher.cs
  * PasswordValidator.cs

* BruteForce

  * BruteForceGenerator.cs

* ViewModels

  * MainWindowViewModel.cs

* Views

  * MainWindow.axaml

### Challenges Encountered

* Duplicate class definitions caused namespace conflicts.
* Incorrect code placement resulted in multiple class declarations.
* Missing namespace imports for generic collections.
* Resolved build errors and successfully restored project functionality.

### Status

Completed:

* Password generation
* SHA256 hashing with static salt
* Password validation
* Brute force combination generation
* Avalonia project setup

Planned for Next Development Session:

* BruteForceEngine implementation
* Single-thread brute force attack
* Multi-thread brute force attack
* CancellationToken support
* Progress tracking
* Elapsed time tracking
* Start/Stop controls
* Performance comparison logging
* UML diagram
* Final report preparation
---

### Version 2.0 – 04 June 2026

### Password Security Implementation

Implemented the password management and security components of the application.

### Completed

#### PasswordHasher

- Implemented SHA256 password hashing.
- Added constant static salt:
  - `COMP123_STATIC_SALT`
- Verified correct hash generation.

#### PasswordManager

- Implemented random password generation.
- Password length generated between `[4–6)` characters as required.
- Integrated password hashing functionality.

#### PasswordValidator

- Implemented password hash validation.
- Separated validation logic from password generation logic.

### Testing

- Generated multiple random passwords.
- Verified generated hashes.
- Confirmed validator correctly compares generated hashes.

### Challenges

- Encountered namespace conflicts caused by duplicate class definitions.
- Resolved build errors related to project structure and missing references.
- Refactored files into appropriate folders.

---

## Version 3.0 – 05 June 2026

### Brute Force Engine Development

Implemented the brute force attack functionality.

### Completed

#### BruteForceGenerator

- Implemented recursive brute force generator.
- Generates combinations from length 1 up to the maximum length.
- Does not require prior knowledge of password length.

#### BruteForceEngine

- Implemented single-thread brute force search.
- Added password recovery using hash comparison.
- Added execution time tracking using `Stopwatch`.

### Testing

Successfully recovered generated passwords.

Example Output:

```text
Original Password: abcd
Found Password: abcd
```

### Challenges

- Managing search performance as password length increased.
- Testing brute force search while maintaining assignment requirements.

---

## Version 4.0 – 06 June 2026

### User Interface Integration

Integrated backend functionality with the Avalonia graphical user interface.

### Completed

#### GUI Features

- Added Generate Password button.
- Added Start Attack button.
- Added Stop Attack button.
- Added Progress Bar.
- Added password display.
- Added elapsed time display.
- Added attack result display.

#### Functionality

- Connected password generation to GUI.
- Connected single-thread brute force engine to GUI.
- Displayed recovered password and execution time.
- Added progress updates within the interface.

### Current Working Features

- Password generation
- SHA256 hashing with static salt
- Password validation
- Brute force generation
- Single-thread brute force attack
- Progress display
- Result display
- Avalonia GUI integration

### Known Issue

The multi-thread brute force implementation has been partially implemented and successfully demonstrates parallel execution using Task-based processing. However, performance testing revealed that the current recursive implementation introduces significant overhead and does not yet outperform the single-thread implementation.

### Planned Improvements

- Redesign multi-thread brute force engine.
- Improve workload distribution across available CPU cores.
- Implement efficient thread cancellation using `CancellationTokenSource`.
- Display multi-thread execution statistics.
- Calculate speedup between single-thread and multi-thread execution.
- Create UML class diagram.
- Prepare final testing report and screenshots.
- Package final project for submission.