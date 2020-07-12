# Portfolio.Contact

| **Branch** | **Status** |
| ---------- | ---------- |
| master | Not configured |

## Description

Portfolio.Contact is a Contact-Me Microservice that allows users who navigate to my portfolio website to send me an email via the contact form. It is built as a Azure Function and uses a HTTP Trigger to processes the email.

The process of this microservice is shown in the below diagram:

![Portfolio.Contact process]()

1. User sends JSON payload via contact form on website, invoking the Contact API endpoint.
2. JSON payload is converted into a SendGridMessage object.
3. SendGridMessage object is sent to SendGrid client.
4. If successful, the Function returns a 200 Status code.
5. If not, the Function returns a 400 Status code.

## Reporting security issues and bugs.

Please raise any issues or bugs by [creating a issue](https://github.com/willvelida/Portfolio.Contact/issues/new).

## Contributing

Details on how to contribute to this project are coming soon!
