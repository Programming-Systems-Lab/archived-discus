package psl.discus.javasrc.shared;

import org.apache.log4j.AppenderSkeleton;
import org.apache.log4j.Layout;
import org.apache.log4j.helpers.LogLog;
import org.apache.log4j.spi.LoggingEvent;

import javax.swing.*;
import java.io.*;

/**
 * This class is used together with the log4j package to provide logging to a swing TextArea component
 * (like the one in UddiP2PTesterGUi).
 * @author matias
 */
public class TextAreaAppender extends AppenderSkeleton {

    private JTextArea textArea;

    public TextAreaAppender() {
    }

    public TextAreaAppender(Layout layout) {
        this.layout = layout;
    }

    public TextAreaAppender(JTextArea textArea) {
        this.textArea = textArea;
    }

    public void append(LoggingEvent event) {
        if (event == null)
            return;

        if (textArea != null) {
            String msg = null;
            if (layout != null )
                msg = layout.format(event);
            else
                msg = (String) event.getMessage();

            textArea.append(msg);
        }
    }


    public boolean requiresLayout() {
        return false;
    }

    public void close() {
    }


}

