# PdfPreflight

Checks PDF files for compliance and issues

## How it works

- PDF is loaded from disk by using the path provided in an argument
- PDF is checked using [Apache PDFBox](https://pdfbox.apache.org/)
- exit codes
  - 0 = PDF is ok
  - 1 = PDF is broken

## Dev notes

- you need JDK 1.8 to compile the application
- you can use IntelliJ IDEA as an IDE
- main method is located in [./src/main/java/com/company/Main.java](./src/main/java/com/company/Main.java)
- for building the application `.jar` you can use [this tutorial](https://www.jetbrains.com/help/idea/compiling-applications.html#package_into_jar)
- can be run with `java -jar PdfPreflight.jar ./myfile.pdf`
  - you need JRE 1.8