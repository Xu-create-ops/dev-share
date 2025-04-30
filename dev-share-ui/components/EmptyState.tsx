"use client";

import { Search } from "lucide-react";
import { Button } from "@/components/ui/button";

interface EmptyStateProps {
  title?: string;
  description?: string;
  searchQuery?: string;
  onSearchSuggestion?: (query: string) => void;
}

export default function EmptyState({
  title = "No resources found",
  description = "Try adjusting your search or browse our trending resources",
  searchQuery,
  onSearchSuggestion
}: EmptyStateProps) {
  // Common developer search terms that might yield results
  const suggestedSearches = ["React", "TypeScript", "Next.js", "JavaScript", "CSS", "Node.js"];
  
  return (
    <div className="col-span-full flex flex-col items-center justify-center py-20 text-center bg-muted/20 rounded-lg border border-dashed border-muted/70 transition-all">
      <div className="flex h-20 w-20 items-center justify-center rounded-full bg-muted/30 mb-6">
        <Search className="h-10 w-10 text-muted-foreground" />
      </div>
      <h3 className="text-xl font-medium mb-2">{title}</h3>
      <p className="text-muted-foreground mb-6 max-w-md">
        {searchQuery ? `No results found for "${searchQuery}". ` : ""}
        {description}
      </p>
      <div className="flex flex-wrap gap-2 justify-center">
        {suggestedSearches.map((term) => (
          <Button
            key={term}
            variant="outline"
            className="px-4 py-2 rounded-full hover:bg-accent hover:text-accent-foreground transition-colors"
            onClick={() => onSearchSuggestion && onSearchSuggestion(term)}
          >
            {term}
          </Button>
        ))}
      </div>
    </div>
  );
}