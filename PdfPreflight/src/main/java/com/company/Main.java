package com.company;

import org.apache.pdfbox.preflight.PreflightDocument;
import org.apache.pdfbox.preflight.ValidationResult;
import org.apache.pdfbox.preflight.exception.SyntaxValidationException;
import org.apache.pdfbox.preflight.parser.PreflightParser;
import java.io.File;
import java.io.IOException;

public class Main {

    static boolean validate_font_integrity(File file) throws IOException {
        ValidationResult result = null;
        try {
            PreflightParser parser = new PreflightParser(file);

            parser.parse();

            PreflightDocument document = parser.getPreflightDocument();

            document.validate();

            result = document.getResult();
            document.close();
        } catch (SyntaxValidationException e) {
            result = e.getResult();
        }

        for (ValidationResult.ValidationError e:
             result.getErrorsList()) {
            System.out.println(e.getDetails());
        }
        // error 3.2.3 - font damaged
        if(result.getErrorsList().stream().anyMatch(e -> e.getErrorCode().equals("3.2.3"))){
            return false;
        }

        return true;
    }

    public static void main(String[] args) {

        File file = new File(args[0]);

        boolean res;

        try {
            res = validate_font_integrity(file);
        } catch (IOException e) {
            e.printStackTrace();

            res = false;
        }

        System.exit(res ? 0 : 1);
    }
}
