"use client";

import { useState, useEffect } from "react";
import { Search } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

interface HeroSectionProps {
  onSearch: (query: string) => void;
  isSearching: boolean;
}

export default function HeroSection({ onSearch, isSearching }: HeroSectionProps) {
  const [query, setQuery] = useState("");
  const [placeholderIndex, setPlaceholderIndex] = useState(0);
  
  const placeholders = [
    "I'm new to React, where should I start?",
    "Best TypeScript tutorials for beginners",
    "How to optimize Next.js performance",
    "GraphQL resources for frontend developers",
    "Learn CSS Grid and Flexbox",
    "Node.js backend architecture patterns"
  ];
  
  // Rotate placeholder text every 5 seconds
  useEffect(() => {
    const interval = setInterval(() => {
      setPlaceholderIndex((prev) => (prev + 1) % placeholders.length);
    }, 5000);
    
    return () => clearInterval(interval);
  }, [placeholders.length]);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (query.trim()) {
      onSearch(query);
    }
  };

  return (
    <section className="w-full py-12 md:py-24 lg:py-32 bg-gradient-to-b from-background to-muted/30">
      <div className="container px-4 md:px-6 mx-auto max-w-5xl text-center">
        <div className="space-y-4 animate-in fade-in slide-in-from-bottom-4 duration-700 fill-mode-both">
          <h1 className="text-3xl font-bold tracking-tighter sm:text-4xl md:text-5xl lg:text-6xl bg-clip-text text-transparent bg-gradient-to-r from-foreground to-foreground/70">
            Find the best developer resources with AI
          </h1>
          <p className="mx-auto max-w-[700px] text-muted-foreground md:text-xl">
            Ask a question, describe what you need, or explore curated resources for developers.
          </p>
        </div>
        
        <div className="mx-auto max-w-2xl mt-8 animate-in fade-in slide-in-from-bottom-8 duration-700 delay-100 fill-mode-both">
          <form onSubmit={handleSubmit} className="flex w-full max-w-2xl items-center space-x-2 mx-auto">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <Input
                type="text"
                placeholder={placeholders[placeholderIndex]}
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                className="pl-10 h-12 rounded-lg bg-card transition-all duration-300 focus:ring-2 focus:ring-primary/20"
                disabled={isSearching}
              />
            </div>
            <Button 
              type="submit" 
              size="lg" 
              disabled={isSearching}
              className="h-12 px-6 transition-all duration-300 hover:shadow-md bg-primary hover:bg-primary/90 hover:scale-[1.02] active:scale-[0.98]"
            >
              {isSearching ? (
                <>
                  <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Finding...
                </>
              ) : (
                "Find Resources"
              )}
            </Button>
          </form>
          
          <p className="mt-3 text-xs text-muted-foreground animate-in fade-in duration-1000 delay-200 fill-mode-both">
            Try: "Best TypeScript tutorials", "How to optimize Next.js", "GraphQL resources"
          </p>
        </div>
      </div>
    </section>
  );
}