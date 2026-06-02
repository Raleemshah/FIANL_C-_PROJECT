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
