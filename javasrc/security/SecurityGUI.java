/*
 * SecurityGUI.java
 *
 * Created on June 10, 2002, 10:44 AM
 */

package psl.discus.javasrc.security;

import org.apache.log4j.BasicConfigurator;
import org.apache.log4j.Logger;

import javax.swing.*;
import javax.swing.tree.DefaultMutableTreeNode;
import javax.swing.tree.DefaultTreeModel;
import java.security.KeyStore;
import java.util.*;
import java.io.*;
import java.awt.event.MouseEvent;
import java.awt.event.ActionEvent;
import java.awt.*;

import psl.discus.javasrc.shared.*;

/**
 *
 * @author  Matias
 */
public class SecurityGUI extends javax.swing.JPanel {

    private static final Logger logger = Logger.getLogger(SecurityGUI.class);

    public static final String PROPS_FILENAME = "SecurityGUI.properties";

    private FakeDataSource ds;
    private ServiceSpaceDAO groupsDAO;
    private JPanel currentPanel;
    private boolean permissionsLoaded;
    private ServicePermissionDAO permissionsDAO;
    private Hashtable groupCache;           // to keep a cache of group id/name tuples
    private boolean newServicePermission;   // for editing/creating new permissions, indicates which
    private int editServicePermissionId;    // for editing permissions
    private boolean connected;
    private Properties props;

    static {
        BasicConfigurator.configure();
    }

    /** Creates new form SecurityGUI */
    public SecurityGUI() {
        initComponents();

        this.setLayout(new GridLayout(1,1));
        this.add(jTabPane);

        props = new Properties();
        File file = new File(PROPS_FILENAME);
        if (file.exists()) {
            try {
                FileInputStream fin = new FileInputStream(file);
                props.load(fin);

                driverClassText.setText(props.getProperty("driverClass"));
                connectionUrlText.setText(props.getProperty("connectionUrl"));
                connectionUsernameText.setText(props.getProperty("username"));
                connectionPasswordText.setText(props.getProperty("password"));

            }
            catch (Exception e) {
                logger.debug("Could not load properties: " + e);
            }
        }


    }

    private void loadServiceSpaces() {

        // we first get the keystore to get the service space list from there
        KeyStore keystore = null;
        String keyStorePassword = "discus";
        try {
            keystore = KeyStore.getInstance(KeyStoreDAO.KEYSTORE_TYPE);
            KeyStoreDAO keyStoreDAO = new KeyStoreDAO(ds);
            keyStoreDAO.loadKeyStore(0,keystore,keyStorePassword.toCharArray());

            // populate service space list
            DefaultListModel model = new DefaultListModel();
            for (Enumeration aliases = keystore.aliases();aliases.hasMoreElements();) {
                String alias = (String) aliases.nextElement();
                ServiceSpaceItem item = new ServiceSpaceItem(Util.parseInt(alias),"Service Space " + alias);
                model.addElement(item);
            }
            serviceSpaceList.setModel(model);

             }
        catch (Exception e) {
            error("Could not load KeyStore");
        }

    }

    private void loadGroups() {

        Vector groups = null;

        try {
            groups = groupsDAO.getGroups();
            groupsList.setListData(groups);

            groupsCombo.setModel(new DefaultComboBoxModel(groups));

            // refresh the groups cache
            groupCache.clear();
            for (Enumeration e = groups.elements();e.hasMoreElements();) {
                Group g = (Group) e.nextElement();
                groupCache.put(new Integer(g.getGroupId()),new GroupNode(g));
            }

        } catch (DAOException e) {
            error("Could not get group list: " + e);
        }

    }

    private void loadServiceSpacesInGroup(int groupid) {

        Vector serviceSpaces = null;
        try {
            serviceSpaces = groupsDAO.getServiceSpacesInGroup(groupid);
            serviceSpaceInGroupList.setListData(serviceSpaces);
        }
        catch (DAOException e) {
            error("Could not get service space list: " + e);
        }

    }

    private void loadPermissions() {

        Vector servicePermissions = null;
        try {
            servicePermissions = permissionsDAO.getAllPermissions();

            DefaultMutableTreeNode root = new DefaultMutableTreeNode("Service Permissions");

            // add a node for each group, regardless of whether it has permissions assigned to it or not
            for (Enumeration e = groupCache.elements();e.hasMoreElements();) {
                GroupNode node = (GroupNode) e.nextElement();
                node.removeAllChildren();   // reset
                root.add(node);
            }

            int currentGroupId = -1;
            DefaultMutableTreeNode currentGroupNode = null;
            for (Enumeration permissions = servicePermissions.elements();permissions.hasMoreElements();) {
                ServicePermission permission = (ServicePermission) permissions.nextElement();
                int groupid = permission.getGroupId();
                if (groupid != currentGroupId) {
                    currentGroupNode = (GroupNode) groupCache.get(new Integer(groupid));
                    currentGroupId = groupid;
                }

                if (currentGroupNode != null) {
                    DefaultMutableTreeNode serviceNode = new DefaultMutableTreeNode(permission.getServiceName());
                    for (Enumeration methods = permission.getMethods().elements();methods.hasMoreElements();) {
                        MethodPermission mp = (MethodPermission) methods.nextElement();
                        DefaultMutableTreeNode methodNode = new MethodNode(mp);
                        serviceNode.add(methodNode);
                    }
                    currentGroupNode.add(serviceNode);
                }
            }

            DefaultTreeModel model = new DefaultTreeModel(root);
            permissionsTree.setModel(model);

        } catch (DAOException e) {
            error("Could not get permissions: " + e);
        }

        permissionsLoaded = true;
    }

    private void loadServices() {

        try {
            Vector services = permissionsDAO.getRegisteredServices();
            serviceNameCombo.setModel(new DefaultComboBoxModel(services));

        } catch (DAOException e) {
            error("Could not get services list: " + e);
        }

        Service service = (Service) serviceNameCombo.getSelectedItem();
        if (service != null) {
            loadMethods(service.getServiceId());
        }

    }

    private void loadMethods(int serviceId) {
        try {
            Vector methods = permissionsDAO.getMethodNames(serviceId);
            methodNameCombo.setModel(new DefaultComboBoxModel(methods));
        } catch (DAOException e) {
            error("Could not get methods list: " + e);
        }
    }

    private void error(String message) {
        logger.debug(message);
        JOptionPane.showMessageDialog(this,message,"Security Manager Admin Error",JOptionPane.WARNING_MESSAGE);
    }

    /** This method is called from within the constructor to
     * initialize the form.
     * WARNING: Do NOT modify this code. The content of this method is
     * always regenerated by the Form Editor.
     */
    private void initComponents() {//GEN-BEGIN:initComponents
        newPermissionPopupMenu = new javax.swing.JPopupMenu();
        newPermissionMenuItem = new javax.swing.JMenuItem();
        editPermissionPopupMenu = new javax.swing.JPopupMenu();
        editPermissionMenuItem = new javax.swing.JMenuItem();
        removePermissionMenuItem = new javax.swing.JMenuItem();
        jTabPane = new javax.swing.JTabbedPane();
        connectionPanel = new javax.swing.JPanel();
        jLabel12 = new javax.swing.JLabel();
        driverClassText = new javax.swing.JTextField();
        jLabel15 = new javax.swing.JLabel();
        connectionUrlText = new javax.swing.JTextField();
        jLabel13 = new javax.swing.JLabel();
        connectionUsernameText = new javax.swing.JTextField();
        jLabel14 = new javax.swing.JLabel();
        connectionPasswordText = new javax.swing.JPasswordField();
        jLabel5 = new javax.swing.JLabel();
        connectButton = new javax.swing.JButton();
        serviceSpacesPanel = new javax.swing.JPanel();
        jPanel2 = new javax.swing.JPanel();
        jLabel1 = new javax.swing.JLabel();
        serviceSpaceListScrollPane = new javax.swing.JScrollPane();
        serviceSpaceList = new javax.swing.JList();
        addServiceSpaceButton = new javax.swing.JButton();
        jPanel6 = new javax.swing.JPanel();
        jPanel5 = new javax.swing.JPanel();
        jPanel4 = new javax.swing.JPanel();
        availableGroupsLabel = new javax.swing.JLabel();
        groupsListScrollPane = new javax.swing.JScrollPane();
        groupsList = new javax.swing.JList();
        newgroupPanel = new javax.swing.JPanel();
        jLabel4 = new javax.swing.JLabel();
        newGroupText = new javax.swing.JTextField();
        removeGroupButton = new javax.swing.JButton();
        jPanel3 = new javax.swing.JPanel();
        jLabel3 = new javax.swing.JLabel();
        serviceSpaceInGroupScrollPane = new javax.swing.JScrollPane();
        serviceSpaceInGroupList = new javax.swing.JList();
        removeServiceSpaceButton = new javax.swing.JButton();
        servicePermissionsPanel = new javax.swing.JPanel();
        permissionsTreePanel = new javax.swing.JPanel();
        jLabel11 = new javax.swing.JLabel();
        jScrollPane4 = new javax.swing.JScrollPane();
        permissionsTree = new javax.swing.JTree();
        permissionPanel = new javax.swing.JPanel();
        jPanel1 = new javax.swing.JPanel();
        jLabel7 = new javax.swing.JLabel();
        groupsCombo = new javax.swing.JComboBox();
        jLabel6 = new javax.swing.JLabel();
        jLabel2 = new javax.swing.JLabel();
        jLabel8 = new javax.swing.JLabel();
        paramsText = new javax.swing.JTextField();
        jLabel9 = new javax.swing.JLabel();
        numInvokationsText = new javax.swing.JTextField();
        jLabel10 = new javax.swing.JLabel();
        methodImplementationText = new javax.swing.JTextField();
        cancelPermissionButton = new javax.swing.JButton();
        addPermissionButton = new javax.swing.JButton();
        serviceNameCombo = new javax.swing.JComboBox();
        methodNameCombo = new javax.swing.JComboBox();

        newPermissionMenuItem.setText("New permission...");
        newPermissionMenuItem.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                newPermissionMenuItemActionPerformed(evt);
            }
        });

        newPermissionPopupMenu.add(newPermissionMenuItem);
        editPermissionMenuItem.setText("Edit permission...");
        editPermissionMenuItem.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                editPermissionMenuItemActionPerformed(evt);
            }
        });

        editPermissionPopupMenu.add(editPermissionMenuItem);
        removePermissionMenuItem.setText("Remove permission...");
        removePermissionMenuItem.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                removePermissionMenuItemActionPerformed(evt);
            }
        });

        editPermissionPopupMenu.add(removePermissionMenuItem);



        jTabPane.setBorder(new javax.swing.border.EmptyBorder(new java.awt.Insets(1, 1, 1, 1)));
        jTabPane.setEnabled(false);
        jTabPane.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                jTabPaneMouseClicked(evt);
            }
        });

        connectionPanel.setLayout(new java.awt.GridBagLayout());
        java.awt.GridBagConstraints gridBagConstraints1;

        jLabel12.setText("Driver:");
        jLabel12.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(jLabel12, gridBagConstraints1);

        driverClassText.setText("org.hsqldb.jdbcDriver");
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.ipady = 6;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(driverClassText, gridBagConstraints1);

        jLabel15.setText("URL:");
        jLabel15.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 0;
        gridBagConstraints1.gridy = 1;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(jLabel15, gridBagConstraints1);

        connectionUrlText.setText("jdbc:hsqldb:db/discus-security");
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 1;
        gridBagConstraints1.gridy = 1;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.ipady = 6;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(connectionUrlText, gridBagConstraints1);

        jLabel13.setText("Username:");
        jLabel13.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 0;
        gridBagConstraints1.gridy = 2;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(jLabel13, gridBagConstraints1);

        connectionUsernameText.setText("sa");
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 1;
        gridBagConstraints1.gridy = 2;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.ipady = 6;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(connectionUsernameText, gridBagConstraints1);

        jLabel14.setText("Password:");
        jLabel14.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 0;
        gridBagConstraints1.gridy = 3;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(jLabel14, gridBagConstraints1);

        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 1;
        gridBagConstraints1.gridy = 3;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.ipadx = 4;
        gridBagConstraints1.ipady = 6;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(connectionPasswordText, gridBagConstraints1);

        gridBagConstraints1 = new java.awt.GridBagConstraints();
        connectionPanel.add(jLabel5, gridBagConstraints1);

        connectButton.setText("Connect");
        connectButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                connectButtonActionPerformed(evt);
            }
        });

        gridBagConstraints1 = new java.awt.GridBagConstraints();
        gridBagConstraints1.gridx = 1;
        gridBagConstraints1.gridy = 4;
        gridBagConstraints1.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints1.insets = new java.awt.Insets(2, 0, 0, 2);
        connectionPanel.add(connectButton, gridBagConstraints1);

        jTabPane.addTab("Connection", connectionPanel);

        serviceSpacesPanel.setLayout(new javax.swing.BoxLayout(serviceSpacesPanel, javax.swing.BoxLayout.Y_AXIS));

        jPanel2.setLayout(new java.awt.BorderLayout());

        jLabel1.setText("Known service spaces:");
        jPanel2.add(jLabel1, java.awt.BorderLayout.NORTH);

        serviceSpaceListScrollPane.setPreferredSize(new java.awt.Dimension(100, 131));
        serviceSpaceList.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        serviceSpaceListScrollPane.setViewportView(serviceSpaceList);

        jPanel2.add(serviceSpaceListScrollPane, java.awt.BorderLayout.CENTER);

        addServiceSpaceButton.setText("Add service space to selected group");
        addServiceSpaceButton.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                addServiceSpaceButtonMouseClicked(evt);
            }
        });

        jPanel2.add(addServiceSpaceButton, java.awt.BorderLayout.SOUTH);

        serviceSpacesPanel.add(jPanel2);

        jPanel6.setPreferredSize(new java.awt.Dimension(10, 20));
        serviceSpacesPanel.add(jPanel6);

        jPanel5.setLayout(new javax.swing.BoxLayout(jPanel5, javax.swing.BoxLayout.X_AXIS));

        jPanel4.setLayout(new java.awt.BorderLayout());

        availableGroupsLabel.setText("Available groups:");
        availableGroupsLabel.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        jPanel4.add(availableGroupsLabel, java.awt.BorderLayout.NORTH);

        groupsList.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        groupsList.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                groupsListMouseClicked(evt);
            }
        });

        groupsListScrollPane.setViewportView(groupsList);

        jPanel4.add(groupsListScrollPane, java.awt.BorderLayout.CENTER);

        newgroupPanel.setLayout(new java.awt.BorderLayout());

        jLabel4.setText("New:");
        newgroupPanel.add(jLabel4, java.awt.BorderLayout.WEST);

        newGroupText.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                newGroupTextActionPerformed(evt);
            }
        });

        newgroupPanel.add(newGroupText, java.awt.BorderLayout.CENTER);

        removeGroupButton.setText("Remove");
        removeGroupButton.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                removeGroupButtonMouseClicked(evt);
            }
        });

        newgroupPanel.add(removeGroupButton, java.awt.BorderLayout.SOUTH);

        jPanel4.add(newgroupPanel, java.awt.BorderLayout.SOUTH);

        jPanel5.add(jPanel4);

        jPanel3.setLayout(new java.awt.BorderLayout());

        jLabel3.setText("Service spaces in group:");
        jPanel3.add(jLabel3, java.awt.BorderLayout.NORTH);

        serviceSpaceInGroupList.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
        serviceSpaceInGroupScrollPane.setViewportView(serviceSpaceInGroupList);

        jPanel3.add(serviceSpaceInGroupScrollPane, java.awt.BorderLayout.CENTER);

        removeServiceSpaceButton.setText("Remove");
        removeServiceSpaceButton.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mouseClicked(java.awt.event.MouseEvent evt) {
                removeServiceSpaceButtonMouseClicked(evt);
            }
        });

        jPanel3.add(removeServiceSpaceButton, java.awt.BorderLayout.SOUTH);

        jPanel5.add(jPanel3);

        serviceSpacesPanel.add(jPanel5);

        jTabPane.addTab("Service Spaces", serviceSpacesPanel);

        servicePermissionsPanel.setLayout(new java.awt.CardLayout());

        servicePermissionsPanel.setBorder(new javax.swing.border.TitledBorder(""));
        permissionsTreePanel.setLayout(new java.awt.BorderLayout());

        jLabel11.setText("Current permissions:");
        permissionsTreePanel.add(jLabel11, java.awt.BorderLayout.NORTH);

        permissionsTree.addMouseListener(new java.awt.event.MouseAdapter() {
            public void mousePressed(java.awt.event.MouseEvent evt) {
                permissionsTreeMousePressed(evt);
            }
        });

        permissionsTree.addTreeSelectionListener(new javax.swing.event.TreeSelectionListener() {
            public void valueChanged(javax.swing.event.TreeSelectionEvent evt) {
                permissionsTreeValueChanged(evt);
            }
        });

        jScrollPane4.setViewportView(permissionsTree);

        permissionsTreePanel.add(jScrollPane4, java.awt.BorderLayout.CENTER);

        servicePermissionsPanel.add(permissionsTreePanel, "permissionsTreePanel");

        permissionPanel.setLayout(new java.awt.BorderLayout());

        jPanel1.setLayout(new java.awt.GridBagLayout());
        java.awt.GridBagConstraints gridBagConstraints2;

        jPanel1.setBorder(new javax.swing.border.TitledBorder(null, "Service Invokation Permission", javax.swing.border.TitledBorder.DEFAULT_JUSTIFICATION, javax.swing.border.TitledBorder.DEFAULT_POSITION, new java.awt.Font("Dialog", 0, 12)));
        jLabel7.setText("Group:");
        jLabel7.setHorizontalAlignment(javax.swing.SwingConstants.RIGHT);
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        jPanel1.add(jLabel7, gridBagConstraints2);

        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridwidth = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 2, 0);
        jPanel1.add(groupsCombo, gridBagConstraints2);

        jLabel6.setText("Service name:");
        jLabel6.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 0;
        gridBagConstraints2.gridy = 1;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 0, 2);
        jPanel1.add(jLabel6, gridBagConstraints2);

        jLabel2.setText("Method name:");
        jLabel2.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 0;
        gridBagConstraints2.gridy = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 0, 2);
        jPanel1.add(jLabel2, gridBagConstraints2);

        jLabel8.setText("Parameters:");
        jLabel8.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 0;
        gridBagConstraints2.gridy = 3;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 0, 2);
        jPanel1.add(jLabel8, gridBagConstraints2);

        paramsText.setMaximumSize(new java.awt.Dimension(2147483647, 16));
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 1;
        gridBagConstraints2.gridy = 3;
        gridBagConstraints2.gridwidth = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 2, 0);
        jPanel1.add(paramsText, gridBagConstraints2);

        jLabel9.setText("# Invokations:");
        jLabel9.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 0;
        gridBagConstraints2.gridy = 4;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 0, 2);
        jPanel1.add(jLabel9, gridBagConstraints2);

        numInvokationsText.setMaximumSize(new java.awt.Dimension(2147483647, 16));
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 1;
        gridBagConstraints2.gridy = 4;
        gridBagConstraints2.gridwidth = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 2, 0);
        jPanel1.add(numInvokationsText, gridBagConstraints2);

        jLabel10.setText("Implementation:");
        jLabel10.setHorizontalAlignment(javax.swing.SwingConstants.LEFT);
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 0;
        gridBagConstraints2.gridy = 5;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 0, 2);
        jPanel1.add(jLabel10, gridBagConstraints2);

        methodImplementationText.setMaximumSize(new java.awt.Dimension(2147483647, 16));
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 1;
        gridBagConstraints2.gridy = 5;
        gridBagConstraints2.gridwidth = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 2, 0);
        jPanel1.add(methodImplementationText, gridBagConstraints2);

        cancelPermissionButton.setText("Cancel");
        cancelPermissionButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                cancelPermissionButtonActionPerformed(evt);
            }
        });

        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 1;
        gridBagConstraints2.gridy = 6;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 2, 0);
        jPanel1.add(cancelPermissionButton, gridBagConstraints2);

        addPermissionButton.setText("Add/Modify");
        addPermissionButton.setPreferredSize(new java.awt.Dimension(97, 27));
        addPermissionButton.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                addPermissionButtonActionPerformed(evt);
            }
        });

        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 2;
        gridBagConstraints2.gridy = 6;
        gridBagConstraints2.insets = new java.awt.Insets(2, 0, 2, 0);
        jPanel1.add(addPermissionButton, gridBagConstraints2);

        serviceNameCombo.setEditable(true);
        serviceNameCombo.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                serviceNameComboActionPerformed(evt);
            }
        });

        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 1;
        gridBagConstraints2.gridy = 1;
        gridBagConstraints2.gridwidth = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        jPanel1.add(serviceNameCombo, gridBagConstraints2);

        methodNameCombo.setEditable(true);
        methodNameCombo.addActionListener(new java.awt.event.ActionListener() {
            public void actionPerformed(java.awt.event.ActionEvent evt) {
                methodNameComboActionPerformed(evt);
            }
        });
        gridBagConstraints2 = new java.awt.GridBagConstraints();
        gridBagConstraints2.gridx = 1;
        gridBagConstraints2.gridy = 2;
        gridBagConstraints2.gridwidth = 2;
        gridBagConstraints2.fill = java.awt.GridBagConstraints.HORIZONTAL;
        jPanel1.add(methodNameCombo, gridBagConstraints2);

        permissionPanel.add(jPanel1, java.awt.BorderLayout.CENTER);

        servicePermissionsPanel.add(permissionPanel, "permissionPanel");

        jTabPane.addTab("Service Permissions", servicePermissionsPanel);


    }//GEN-END:initComponents


    private void serviceNameComboActionPerformed(ActionEvent evt) {
        // when user chooses a service, update the list of available methods

        Object o = (Object) serviceNameCombo.getSelectedItem();
        if (o instanceof Service) {
            Service service = (Service) o;
            if (service != null) {
                loadMethods(service.getServiceId());
            }
        }
    }

    private void methodNameComboActionPerformed(ActionEvent evt) {
        // when user chooses a method, fill in a default methodImplemention

        String methodName = (String) methodNameCombo.getSelectedItem();
        methodImplementationText.setText(methodName);
    }

    private void connectButtonActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_connectButtonActionPerformed

        if (connected) {

            try {
                ds.setPersistConnection(false);
            } catch (DAOException e) {
                error("Problem closing connection: " + e);
            }

            connectButton.setText("Connect");
            connected = false;
            jTabPane.setEnabled(false);

        }
        else {

            // save settings
            props.setProperty("driverClass",driverClassText.getText());
            props.setProperty("connectionUrl",connectionUrlText.getText());
            props.setProperty("username",connectionUsernameText.getText());
            props.setProperty("password",new String(connectionPasswordText.getPassword()));

            File file = new File(PROPS_FILENAME);
            try {
                FileOutputStream out = new FileOutputStream(file);
                props.store(out,"SecurityGUI properties");
            }
            catch (IOException e) {
                logger.debug("Could not save settings");
            }

            ds = new FakeDataSource(props);

            try {
                ds.setPersistConnection(true);  // so that we use only one connection per instance
            }
            catch (DAOException e) {
                error("Could not open connection to database: " + e);
                return;
            }

            groupsDAO = new ServiceSpaceDAO(ds);
            permissionsDAO = new ServicePermissionDAO(ds);

            groupCache = new Hashtable();

            loadServiceSpaces();
            loadGroups();
            loadPermissions();
            loadServices();

            connected = true;
            connectButton.setText("Disconnect");
            jTabPane.setEnabled(true);
            jTabPane.setSelectedComponent(serviceSpacesPanel);

        }



    }//GEN-LAST:event_connectButtonActionPerformed

    private void cancelPermissionButtonActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_cancelPermissionButtonActionPerformed

        // TODO prompt for confirmation
        showCard("permissionsTreePanel");

    }//GEN-LAST:event_cancelPermissionButtonActionPerformed

    private void addPermissionButtonActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_addPermissionButtonActionPerformed

        String serviceName = null;
        // since the combobox is editable, sometimes we may get a Service and sometimes a String
        Object o = (Object) serviceNameCombo.getSelectedItem();
        if (o instanceof Service) {
            serviceName = ((Service) o).getServiceName();
        }
        else {
            serviceName = (String) o;
        }

        String methodName = (String) methodNameCombo.getSelectedItem();

        if (serviceName == null || methodName == null || empty(methodImplementationText) ||
                empty(numInvokationsText)) {
            error("Please fill in the service name, method name, and number of invokations.");
            return;
        }

        int numInvokations = Util.parseInt(numInvokationsText.getText());
        if (numInvokations == -1) {
            error("Please enter a valid number of invokations allowed.");
            return;
        }

        int groupid = ((Group)groupsCombo.getSelectedItem()).getGroupId();

        if (newServicePermission) {
            // add this permission
            try {
                permissionsDAO.addPermission(groupid,serviceName,methodName,
                    paramsText.getText(),numInvokations,methodImplementationText.getText());
            } catch (DAOException e) {
                error("Could not add permission:\n " + e);
            }
        }
        else {
            // modify this permission
            try {
                permissionsDAO.modifyPermission(editServicePermissionId, groupid,serviceName,
                    methodName,paramsText.getText(),numInvokations,methodImplementationText.getText());
            } catch (DAOException e) {
                error("Could not modify permission:\n " + e);
            }
        }

        loadPermissions();  // refresh

        showCard("permissionsTreePanel");

    }//GEN-LAST:event_addPermissionButtonActionPerformed

    private boolean empty(JTextField serviceNameText) {
        return (serviceNameText.getText().length() == 0);
    }


    private void removePermissionMenuItemActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_removePermissionMenuItemActionPerformed

        DefaultMutableTreeNode node = (DefaultMutableTreeNode) permissionsTree.getLastSelectedPathComponent();
        if (node == null || !(node instanceof MethodNode))
            return;

        int confirm = JOptionPane.showConfirmDialog(this,
                "Are you sure you want to delete the selected permission?", "Confirmation",
                JOptionPane.YES_NO_OPTION,JOptionPane.WARNING_MESSAGE);

        if (confirm == JOptionPane.NO_OPTION)
            return;

        MethodPermission mp = ((MethodNode) node).getMethodPermission();

        try {
            permissionsDAO.removePermissionForMethod(mp.getPermissionId());
            loadPermissions();
        }
        catch (DAOException e) {
            error("Could not remove permission: " + e);
        }

    }//GEN-LAST:event_removePermissionMenuItemActionPerformed


    private void permissionsTreeMousePressed(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_permissionsTreeMousePressed

        DefaultMutableTreeNode node = (DefaultMutableTreeNode) permissionsTree.getLastSelectedPathComponent();
        if (node == null)
            return;
        else if ((evt.getModifiers() & MouseEvent.BUTTON3_MASK) == MouseEvent.BUTTON3_MASK) {
             if (node instanceof GroupNode) {
                 newPermissionPopupMenu.show(permissionsTree,evt.getX(),evt.getY());
             }
             else if (node instanceof MethodNode) {
                 editPermissionPopupMenu.show(permissionsTree, evt.getX(), evt.getY());
             }
        }

    }//GEN-LAST:event_permissionsTreeMousePressed

    private void newGroupTextActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_newGroupTextActionPerformed
        String name = newGroupText.getText();
            if (name != null && name.length() > 1) {
                try {
                    groupsDAO.addGroup(name);
                    loadGroups();
                    newGroupText.setText("");
                }
                catch (DAOException e) {
                    error("Could not add group: " + e);
                }
            }
    }//GEN-LAST:event_newGroupTextActionPerformed

    private void newPermissionMenuItemActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_newPermissionMenuItemActionPerformed

        DefaultMutableTreeNode node = (DefaultMutableTreeNode ) permissionsTree.getLastSelectedPathComponent();
        if (node == null || !(node instanceof GroupNode))
            return;

        groupsCombo.setSelectedItem(((GroupNode)node).getGroup());

        paramsText.setText("");
        numInvokationsText.setText("");
        methodImplementationText.setText("");
        newServicePermission = true;

        showCard("permissionPanel");

    }//GEN-LAST:event_newPermissionMenuItemActionPerformed

    private void editPermissionMenuItemActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_editPermissionMenuItemActionPerformed

        DefaultMutableTreeNode node = (DefaultMutableTreeNode ) permissionsTree.getLastSelectedPathComponent();
        if (node == null || !(node instanceof MethodNode))
            return;

        DefaultMutableTreeNode serviceNode = (DefaultMutableTreeNode) node.getParent();

        GroupNode groupNode = (GroupNode) serviceNode.getParent();
        groupsCombo.setSelectedItem(groupNode.getGroup());

        MethodPermission mp = ((MethodNode) node).getMethodPermission();

        // unfortunately we have to go through the list and find the service object that matches
        // the chosen service
        String chosenService = (String) serviceNode.getUserObject();
        boolean found = false;
        for (int i=0;i<serviceNameCombo.getItemCount();i++) {
            Service service = (Service) serviceNameCombo.getItemAt(i);
            if (service.getServiceName().equals(chosenService)) {
                serviceNameCombo.setSelectedIndex(i);
                found = true;
                break;
            }
        }
        if (!found)
            serviceNameCombo.setSelectedIndex(0);

        methodNameCombo.setSelectedItem(mp.getMethodName());
        paramsText.setText(Util.collectionToString(mp.getParams()));
        numInvokationsText.setText(String.valueOf(mp.getNumberInvokations()));
        methodImplementationText.setText(mp.getMethodImplementation());
        newServicePermission = true;

        newServicePermission = false;
        editServicePermissionId = mp.getPermissionId();

        showCard("permissionPanel");

    }//GEN-LAST:event_editPermissionMenuItemActionPerformed


    private void showCard(String cardName) {
        CardLayout cl = (CardLayout) servicePermissionsPanel.getLayout();
        cl.show(servicePermissionsPanel,cardName);
    }

    private void permissionsTreeValueChanged(javax.swing.event.TreeSelectionEvent evt) {//GEN-FIRST:event_permissionsTreeValueChanged
        // Add your handling code here:
    }//GEN-LAST:event_permissionsTreeValueChanged

    private void jTabPaneMouseClicked(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_jTabPaneMouseClicked



    }//GEN-LAST:event_jTabPaneMouseClicked

    private void removeGroupButtonMouseClicked(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_removeGroupButtonMouseClicked

        Group group = (Group) groupsList.getSelectedValue();
        if (group == null)
            return;

        int confirm = JOptionPane.showConfirmDialog(this,
                "Are you sure you want to delete group " + group.getName() + " and all its associated " +
                " permissions?", "Confirmation",
                JOptionPane.YES_NO_OPTION,JOptionPane.WARNING_MESSAGE);

        if (confirm == JOptionPane.NO_OPTION)
            return;

        try {
            groupsDAO.removeGroup(group.getGroupId());
            loadGroups();
        }
         catch (DAOException e) {
            error("Could not delete group: " + e);
        }


    }//GEN-LAST:event_removeGroupButtonMouseClicked

    private void connectionSettingsMenuItemActionPerformed(java.awt.event.ActionEvent evt) {//GEN-FIRST:event_connectionSettingsMenuItemActionPerformed
        // Add your handling code here:
    }//GEN-LAST:event_connectionSettingsMenuItemActionPerformed

    private void removeServiceSpaceButtonMouseClicked(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_removeServiceSpaceButtonMouseClicked

        ServiceSpace serviceSpace = (ServiceSpace) serviceSpaceInGroupList.getSelectedValue();
        Group group = (Group) groupsList.getSelectedValue();
        if (serviceSpace == null || group == null)
            return;

        try {
            groupsDAO.removeServiceSpaceFromGroup(serviceSpace.getServiceSpaceId(), group.getGroupId());
            loadServiceSpacesInGroup(group.getGroupId());
        }
        catch (DAOException e) {
            error("Could not remove service space from group: " + e);
        }

    }//GEN-LAST:event_removeServiceSpaceButtonMouseClicked

    private void groupsListMouseClicked(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_groupsListMouseClicked

        Group group = (Group) groupsList.getSelectedValue();
        if (group != null)
            loadServiceSpacesInGroup(group.getGroupId());

    }//GEN-LAST:event_groupsListMouseClicked

    private void addServiceSpaceButtonMouseClicked(java.awt.event.MouseEvent evt) {//GEN-FIRST:event_addServiceSpaceButtonMouseClicked

        ServiceSpaceItem item = (ServiceSpaceItem) serviceSpaceList.getSelectedValue();
        Group group = (Group) groupsList.getSelectedValue();

        if (item == null || group == null)
            return;

        try {
            groupsDAO.addServiceSpaceToGroup(item.getServiceSpaceId(),group.getGroupId());
        }
        catch (DAOException e) {
            error("Could not add service space to group: " + e);
        }

        loadServiceSpacesInGroup(group.getGroupId());

    }//GEN-LAST:event_addServiceSpaceButtonMouseClicked

    /** Exit the Application */
    private void exitForm(java.awt.event.WindowEvent evt) {//GEN-FIRST:event_exitForm
        try {
            if (ds != null)
                ds.setPersistConnection(false);     // closes the connection
        }
        catch (DAOException e) {
        }

        System.exit(0);
    }//GEN-LAST:event_exitForm

    /**
    * @param args the command line arguments
    */
    public static void main(String args[]) {

        final SecurityGUI gui = new SecurityGUI();

        JFrame frame = new JFrame();
        frame.getContentPane().setLayout(new java.awt.GridLayout(1, 1));
        frame.getContentPane().add(gui);

        frame.setTitle("Security Manager administration");
        frame.addWindowListener(new java.awt.event.WindowAdapter() {
              public void windowClosing(java.awt.event.WindowEvent evt) {
                gui.exitForm(evt);
              }
        });

        frame.pack();
        frame.show();


    }


    // Variables declaration - do not modify//GEN-BEGIN:variables
    private javax.swing.JPopupMenu newPermissionPopupMenu;
    private javax.swing.JMenuItem newPermissionMenuItem;
    private javax.swing.JPopupMenu editPermissionPopupMenu;
    private javax.swing.JMenuItem editPermissionMenuItem;
    private javax.swing.JMenuItem removePermissionMenuItem;
    private javax.swing.JTabbedPane jTabPane;
    private javax.swing.JPanel connectionPanel;
    private javax.swing.JLabel jLabel12;
    private javax.swing.JTextField driverClassText;
    private javax.swing.JLabel jLabel15;
    private javax.swing.JTextField connectionUrlText;
    private javax.swing.JLabel jLabel13;
    private javax.swing.JTextField connectionUsernameText;
    private javax.swing.JLabel jLabel14;
    private javax.swing.JPasswordField connectionPasswordText;
    private javax.swing.JLabel jLabel5;
    private javax.swing.JButton connectButton;
    private javax.swing.JPanel serviceSpacesPanel;
    private javax.swing.JPanel jPanel2;
    private javax.swing.JLabel jLabel1;
    private javax.swing.JScrollPane serviceSpaceListScrollPane;
    private javax.swing.JList serviceSpaceList;
    private javax.swing.JButton addServiceSpaceButton;
    private javax.swing.JPanel jPanel6;
    private javax.swing.JPanel jPanel5;
    private javax.swing.JPanel jPanel4;
    private javax.swing.JLabel availableGroupsLabel;
    private javax.swing.JScrollPane groupsListScrollPane;
    private javax.swing.JList groupsList;
    private javax.swing.JPanel newgroupPanel;
    private javax.swing.JLabel jLabel4;
    private javax.swing.JTextField newGroupText;
    private javax.swing.JButton removeGroupButton;
    private javax.swing.JPanel jPanel3;
    private javax.swing.JLabel jLabel3;
    private javax.swing.JScrollPane serviceSpaceInGroupScrollPane;
    private javax.swing.JList serviceSpaceInGroupList;
    private javax.swing.JButton removeServiceSpaceButton;
    private javax.swing.JPanel servicePermissionsPanel;
    private javax.swing.JPanel permissionsTreePanel;
    private javax.swing.JLabel jLabel11;
    private javax.swing.JScrollPane jScrollPane4;
    private javax.swing.JTree permissionsTree;
    private javax.swing.JPanel permissionPanel;
    private javax.swing.JPanel jPanel1;
    private javax.swing.JLabel jLabel7;
    private javax.swing.JComboBox groupsCombo;
    private javax.swing.JLabel jLabel6;
    private javax.swing.JLabel jLabel2;
    private javax.swing.JLabel jLabel8;
    private javax.swing.JTextField paramsText;
    private javax.swing.JLabel jLabel9;
    private javax.swing.JTextField numInvokationsText;
    private javax.swing.JLabel jLabel10;
    private javax.swing.JTextField methodImplementationText;
    private javax.swing.JButton cancelPermissionButton;
    private javax.swing.JButton addPermissionButton;
    private javax.swing.JComboBox serviceNameCombo;
    private javax.swing.JComboBox methodNameCombo;
    // End of variables declaration//GEN-END:variables

    private class ServiceSpaceItem {
        int serviceSpaceId;
        String description;

        public ServiceSpaceItem(int serviceSpaceId, String description) {
            this.serviceSpaceId = serviceSpaceId;
            this.description = description;
        }

        public int getServiceSpaceId() {
            return serviceSpaceId;
        }

        public String getDescription() {
            return description;
        }

        public String toString() {
            return description;
        }
    }


    private class GroupNode extends DefaultMutableTreeNode {

        private Group group;

        public GroupNode(Group group) {
            super(group.getName());
            this.group = group;
        }

        public Group getGroup() {
                    return group;
                }


    }

    private class MethodNode extends DefaultMutableTreeNode {
        MethodPermission mp;

        public MethodNode(MethodPermission mp) {
            super(mp.getMethodName());
            this.mp = mp;

            add(new DefaultMutableTreeNode("Params: " + mp.getParams()));
            add(new DefaultMutableTreeNode("# invokations: " + mp.getNumberInvokations()));
            add(new DefaultMutableTreeNode("Implementation: " + mp.getMethodImplementation()));

        }

        public MethodPermission getMethodPermission() {
            return mp;
        }


    }


}
