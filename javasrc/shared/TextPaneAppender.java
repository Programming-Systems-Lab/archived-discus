package psl.discus.javasrc.shared;

import org.apache.log4j.*;
import org.apache.log4j.helpers.LogLog;
import org.apache.log4j.spi.LoggingEvent;

import javax.swing.*;
import javax.swing.text.*;
import java.util.*;

/**
 * This class is used together with the log4j package to provide logging to a swing JTextPane component
 * Different levels will be printed out using different styles, as defined in the JTextPane.
 *
 * @author matias
 */
public class TextPaneAppender extends AppenderSkeleton {

    // if we can't find a style, use this one
    private static Style defaultStyle = StyleContext.getDefaultStyleContext().getStyle(StyleContext.DEFAULT_STYLE);

    private Document doc;
    private HashMap styles;


    public TextPaneAppender(JTextPane textPane) {
        doc = textPane.getDocument();

        styles = new HashMap(4);

        styles.put(Level.DEBUG,textPane.getStyle("debug"));
        styles.put(Level.INFO,textPane.getStyle("info"));
        styles.put(Level.WARN,textPane.getStyle("warn"));
        styles.put(Level.FATAL,textPane.getStyle("fatal"));
    }

    public void append(LoggingEvent event) {
        if (event == null)
            return;

        String msg = null;
        if (layout != null )
            msg = layout.format(event);
        else
            msg = (String) event.getMessage();

        if (doc != null) {
            try {
                Style style = (Style) styles.get(event.level);
                if (style == null)
                    style = defaultStyle;
                doc.insertString(doc.getLength(),msg,style);

            } catch (BadLocationException e) {
                LogLog.warn("problem appending to text: " + e);
                LogLog.debug(msg);
            }
        }
        else {
            LogLog.debug(msg);
        }
    }


    public boolean requiresLayout() {
        return false;
    }

    public void close() {
    }


}

