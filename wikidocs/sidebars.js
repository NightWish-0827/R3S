/** @type {import('@docusaurus/plugin-content-docs').SidebarsConfig} */
const sidebars = {
  docs: [
    {
      type: 'category',
      label: 'ABOUT',
      collapsed: false,
      items: ['intro'],
    },
    {
      type: 'category',
      label: 'Getting Started',
      collapsed: false,
      items: ['getting-started/installation', 'getting-started/quickstart'],
    },
    {
      type: 'category',
      label: 'Descriptions',
      collapsed: false,
      items: ['core/attributes', 'core/generated-code'],
    },
    {
      type: 'category',
      label: 'Trouble Shooting',
      collapsed: false,
      items: [
        {
          type: 'doc',
          id: 'core/diagnostics',
          label: '문제 해결',
        },
      ],
    },
  ],
};

module.exports = sidebars;
